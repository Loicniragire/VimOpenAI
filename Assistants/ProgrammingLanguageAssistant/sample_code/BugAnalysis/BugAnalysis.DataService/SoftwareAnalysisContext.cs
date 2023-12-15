using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BugAnalysis.DataService
{
    public partial class SoftwareAnalysisContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "dbo";


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                //.AddEnvironmentVariables()

                var conn = configuration["ConnectionString"];
                optionsBuilder.UseSqlServer(conn, conf =>
                        {
                            conf.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                base.OnConfiguring(optionsBuilder);
            }
        }
    }
}
