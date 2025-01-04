using Microsoft.OpenApi.Models;

namespace ClinicalTrial.App.Extensions
{
    public static class AppExtensions
    {
        public static IServiceCollection AddAppExtensions(this IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new TrialStatusConverter());
                });
            services.AddEndpointsApiExplorer();

            // Add Swagger/OpenAPI services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Clinical Trial API",
                    Version = "v1",
                    Description = "An API for managing clinical trials.",
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
