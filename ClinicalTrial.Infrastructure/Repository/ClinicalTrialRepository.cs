using ClinicalTrial.Domain.Abstractions;
using ClinicalTrial.Infrastructure.MSSqlDatabase;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrial.Infrastructure.Repository
{
    public class ClinicalTrialRepository : IRepository<Domain.Entities.ClinicalTrial>
    {
        private readonly ClinicalTrialDbContext _dbContext;

        public ClinicalTrialRepository(ClinicalTrialDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateAsync(Domain.Entities.ClinicalTrial trialDTO)
        {
            _dbContext.ClinicalTrials.Add(trialDTO);
            await _dbContext.SaveChangesAsync();
            return trialDTO.Id;
        }

        public async Task<Domain.Entities.ClinicalTrial> GetByIdAsync(Guid id)
        {
            return await _dbContext.ClinicalTrials
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Domain.Entities.ClinicalTrial>> GetAllFilteredAsync(string? status = null, int? minParticipants = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.ClinicalTrials.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }
            if (minParticipants.HasValue)
            {
                query = query.Where(t => t.Participants >= minParticipants.Value);
            }
            if (startDate.HasValue)
            {
                query = query.Where(t => t.StartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(t => t.EndDate <= endDate.Value);
            }

            return await query.ToListAsync();
        }
    }
}
