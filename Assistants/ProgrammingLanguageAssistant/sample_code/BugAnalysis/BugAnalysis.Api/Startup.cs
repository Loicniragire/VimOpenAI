using System;
using Kimado.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BugAnalysis.Api;
using Microsoft.AspNetCore.Authentication.Certificate;
using Kimado.Common.Contracts;
using FluentValidation.AspNetCore;
using FluentValidation;
using BugAnalysis.Api.Models.Requests;
using BugAnalysis.Api.Validators;

namespace BugAnalysis.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "kimado.api", Version = "v1" });
            });
            services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                             .SetIsOriginAllowed((host) => true)
                             .AllowAnyMethod()
                             .AllowAnyHeader()
                             .AllowCredentials()
                            );
                });

            var container = ServiceContainer.Instance;
            container.CacheServices.Register<IMapper>(() => new AutoMapperConfig());

            var mapper = container.CacheServices.Instance<IMapper>();
            mapper.Add(typeof(IMapper), new Mapper(CreateConfiguration()));



            ServiceConfig.Initialize(container);
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                    .AddCertificate(options => Auth(options, container));

			// FluentValidatio
			services.AddFluentValidationAutoValidation();
			services.AddTransient<IValidator<FooRequest>, FooRequestValidator>();
        }

        public static MapperConfiguration CreateConfiguration()
		{
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    /* cfg.CreateMap<Students.Api.Models.Student, Students.FunctionalService.Models.Student>() */
                    /* .ForMember(dest => dest.Firstname, act => act.MapFrom(src => src.Firstname)) */
                    /* .ReverseMap(); */
                });

                config.AssertConfigurationIsValid();
                return config;

            }
            catch(Exception)
            {
                throw;
            }

		}

        private void Auth(CertificateAuthenticationOptions options, Kimado.Core.Contracts.IServiceContainer container)
        {
            options.AllowedCertificateTypes = CertificateTypes.All;
            options.Events = new CertificateAuthenticationEvents
            {
                OnCertificateValidated = context =>
                {
                    var validationService = container.FunctionalServices.Instance<ICertificateService>();
                    if (validationService.GetCertificateBySubjectName(Configuration["Certificate"]) != null)
                    {
                        var claims = new[]
                        {
                                new Claim(ClaimTypes.NameIdentifier, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                new Claim(ClaimTypes.Name, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                        };
                        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                        context.Success();
                    }
                    else
                    {
                        context.Fail($"Invalid Certificate{Configuration["Certificate"]}");
                    }
                    
                    return Task.CompletedTask;
                }
            };
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "kimado.api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
