using ClinicalTrial.Domain.Abstractions;
using ClinicalTrial.Infrastructure.MSSqlDatabase;
using ClinicalTrial.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using static ClinicalTrial.Infrastructure.SerilogConfig.SerilogConfig;

namespace ClinicalTrial.Infrastructure.DependencyExtension
{
    public static class InfrastructureExtensions
    {
        public static void AddSerilog(this IHostBuilder builder)
        {
            Log.Logger = SetupSerilog();
            builder.UseSerilog();
        }

        public static IServiceCollection RegisterInfrastructureDependencies(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IRepository<Domain.Entities.ClinicalTrial>, ClinicalTrialRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddDbContext<ClinicalTrialDbContext>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}
