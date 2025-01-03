using ClinicalTrial.Application.CQRS.Queries;
using ClinicalTrial.Domain.Abstractions;
using ClinicalTrial.Domain.Entities;
using ClinicalTrial.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ClinicalTrial.Application.CQRS.Handlers.Queries
{
    public class GetClinicalTrialByIdQueryHandler : IRequestHandler<GetClinicalTrialByIdQuery, Domain.Entities.ClinicalTrial>
    {
        private readonly IRepository<Domain.Entities.ClinicalTrial> _clinicalTrialRepository;
        private readonly ILogger<GetClinicalTrialByIdQueryHandler> _logger;

        public GetClinicalTrialByIdQueryHandler(IRepository<Domain.Entities.ClinicalTrial> clinicalTrialRepository, ILogger<GetClinicalTrialByIdQueryHandler> logger)
        {
            _clinicalTrialRepository = clinicalTrialRepository;
            _logger = logger;
        }

        public async Task<Domain.Entities.ClinicalTrial> Handle(GetClinicalTrialByIdQuery request, CancellationToken cancellationToken)
        {
            var ClinicalTrial = await _clinicalTrialRepository.GetByIdAsync(request.Id);

            _logger.LogInformation("{@SystemLog}", new SystemLog()
            {
                Event = Event.Read,
                Comment = "Clinical Trial by Id",
                CreatedAt = DateTime.UtcNow,
                ChangeSet = new[] { request }
            });

            return ClinicalTrial;
        }
    }
}
