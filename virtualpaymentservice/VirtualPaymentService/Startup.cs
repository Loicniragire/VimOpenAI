using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProgLeasing.Platform.SecretConfiguration.Extensions;
using ProgLeasing.System.Data.Contract;
using ProgLeasing.System.FeatureFlag.LaunchDarkly.Core.Extensions;
using ProgLeasing.System.Logging;
using ProgLeasing.System.Logging.Correlator;
using ProgLeasing.System.Logging.Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using VirtualPaymentService.Business.Common.AutoMapper;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Factory;
using VirtualPaymentService.Business.Factory.Interface;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Provider;
using VirtualPaymentService.Business.Service;
using VirtualPaymentService.Data;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Data.Facade;
using VirtualPaymentService.Data.Interface;
using VirtualPaymentService.Exceptions;
using VirtualPaymentService.HealthChecks;

namespace VirtualPaymentService
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(appSettings);
            services.AddSingleton(appSettings);

            services.AddRequestCorrelation();

            // HttpClients
            services
                .AddHttpClient<IMarqetaCommercialService, MarqetaCommercialService>()
                .AddCorrelationHandler();

            services
                .AddHttpClient<IMarqetaConsumerService, MarqetaConsumerService>()
                .AddCorrelationHandler();

            //For Proglogger
            services.AddHttpContextAccessor();
            services.AddProgLogger(Configuration, builder =>
            {
                // Register the IConfigureLoggerProvider implementation for Serilog 
                builder.RegisterSerilog();
                
                var customProperties = new Dictionary<string, string>
                {
                    { "BuildVersion", GetBuildVersion() }
                };
                
                builder.ApplicationScopeOptions.CustomProperties = customProperties;
            });

            services.AddApplicationInsightsTelemetry();
            services.AddLaunchDarklyFeatureFlagDependencies(Configuration);

            // See link for list of available health checks:
            // https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks#health-checks
            services.AddHealthChecks()
                .AddCheck<VCardProviderHealthCheck>("vcard_provider_ping_checks");

            // Database connection setup
            if (!int.TryParse(Configuration["Database:CommandTimeout"], out int commandTimeout))
            {
                commandTimeout = 30;
            }
            var databaseInfos = new Dictionary<string, DatabaseInfo>()
                {
                    {
                        Constants.ProgressiveDatabase,
                        new DatabaseInfo {
                            ConnectionString = Configuration.GetConnectionString(Constants.ProgressiveDatabase),
                            CommandTimeout = commandTimeout,
                            DatabaseName = Constants.ProgressiveDatabase,
                            DatabaseProvider = DatabaseProvider.SqlServer
                        }
                    },
                    {
                        Constants.VirtualPaymentDatabase,
                        new DatabaseInfo {
                            ConnectionString = Configuration.GetConnectionString(Constants.VirtualPaymentDatabase),
                            CommandTimeout = commandTimeout,
                            DatabaseName = Constants.VirtualPaymentDatabase,
                            DatabaseProvider = DatabaseProvider.SqlServer
                        }
                    }
                };

            //Dependency Injection
            services.AddAutoMapper(typeof(DigitalWalletProfile), typeof(VirtualCardProfile));
            services.AddSecretManager(Configuration);
            services.AddSingleton(databaseInfos);
            services.AddScoped<IVirtualCardProvider, VirtualCardProvider>();
            services.AddScoped<IVPayTransactionProvider, VPayTransactionProvider>();
            services.AddScoped<IJITDecisionProvider, JITDecisionProvider>();
            services.AddScoped<ILeaseStoreDataProvider, LeaseStoreDataProvider>();
            services.AddScoped<IVirtualPaymentRepository, VirtualPaymentRepository>();
            services.AddTransient<ICardProviderFactory, CardProviderFactory>();
            services.AddTransient<IUnitOfWorkFactory, TransactionScopeUnitOfWorkFactory>();
            services.AddSingleton<IJITDecisionLogRepository, JITDecisionLogRepository>();
            services.AddSingleton<ILeaseStoreRepository, LeaseStoreRepository>();
            services.AddSingleton<IProviderRepository, ProviderRepository>();
            services.AddSingleton<IVCardRepository, VCardRepository>();
            services.AddSingleton<IVCardPurchaseAuthRepository, VCardPurchaseAuthRepository>();
            services.AddSingleton<IStoreVPayReconciliationRepository, StoreVPayReconciliationRepository>();
            services.AddSingleton<IVPayTransactionRepository, VPayTransactionRepository>();
            services.AddSingleton<IWalletBatchConfigRepository, WalletBatchConfigRepository>();
            services.AddSingleton<IVCardProviderProductTypeRepository, VCardProviderProductTypeRepository>();
            services.AddSingleton<ISecretConfigurationService, SecretConfigurationService>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Virtual Payment Service", Version = "v1" });

                // Uses extension to display mapping of enums in Swagger page
                c.AddEnumsWithValuesFixFilters();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.Model.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //Configure Controllers with Fluent Validation
            services.AddControllers()
                .ConfigureApiBehaviorOptions(opt =>
                {
                    // Must be false to execute validation in controller binding
                    opt.SuppressModelStateInvalidFilter = false;
                    // Modifying ModelState Validation Error Works with this delegate rather than an ActionFilter
                    opt.InvalidModelStateResponseFactory = r =>
                    {
                        var errors = r.ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                        return new BadRequestObjectResult(errors);
                    };
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRequestCorrelation();
            app.ConfigureUnhandledExceptionMiddleware();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Health check - This one used by Octopus Deploy. /health still works as well.
            app.UseHealthChecks("/api/health");
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{(string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..")}/swagger/v1/swagger.json", "API V1");
                c.DefaultModelsExpandDepth(-1);
            });
        }
        
        private static string GetBuildVersion()
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
            
            return version;
        }
    }
}
