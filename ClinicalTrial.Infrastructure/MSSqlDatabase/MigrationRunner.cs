using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalTrial.Infrastructure.MSSqlDatabase
{
    public static class MigrationRunner
    {
        public static void ApplyEFMigrations(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ClinicalTrialDbContext>();
                db.Database.Migrate();
            }
        }
    }
}
