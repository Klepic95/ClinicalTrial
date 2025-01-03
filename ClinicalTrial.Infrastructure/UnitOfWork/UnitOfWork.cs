using ClinicalTrial.Domain.Abstractions;
using ClinicalTrial.Infrastructure.MSSqlDatabase;

namespace ClinicalTrial.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicalTrialDbContext _dbContext;

        public UnitOfWork(ClinicalTrialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
