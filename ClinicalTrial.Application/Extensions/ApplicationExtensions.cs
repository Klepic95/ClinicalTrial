using Microsoft.Extensions.DependencyInjection;

namespace ClinicalTrial.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(typeof(ApplicationAssembly).Assembly));
        }
    }
}