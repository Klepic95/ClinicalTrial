using ClinicalTrial.Application.CQRS.Queries;
using ClinicalTrial.Domain.Abstractions;
using ClinicalTrial.Domain.Entities;
using ClinicalTrial.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ClinicalTrial.Application.CQRS.Handlers.Queries
{
    public class GetFilteredClinicalTrialsQueryHandler : IRequestHandler<GetFilteredClinicalTrialsQuery, IEnumerable<Domain.Entities.ClinicalTrial>>
    {

        private readonly IRepository<Domain.Entities.ClinicalTrial> _clinicalTrialRepositrory;
        private readonly ILogger<GetFilteredClinicalTrialsQueryHandler> _logger;

        public GetFilteredClinicalTrialsQueryHandler(IRepository<Domain.Entities.ClinicalTrial> clinicalTrialRepositrory, ILogger<GetFilteredClinicalTrialsQueryHandler> logger)
        {
            _clinicalTrialRepositrory = clinicalTrialRepositrory;
            _logger = logger;
        }

        public async Task<IEnumerable<Domain.Entities.ClinicalTrial>> Handle(GetFilteredClinicalTrialsQuery request, CancellationToken cancellationToken)
        {
            var clinicalTrials = await _clinicalTrialRepositrory.GetAllFilteredAsync(request.status, request.minParticipants, request.startDate, request.endDate);
            _logger.LogInformation("{@SystemLog}", new SystemLog()
            {
                Event = Event.Read,
                Comment = "Filtered Clinical Trials read",
                CreatedAt = DateTime.UtcNow,
                ChangeSet = new[] { request }
            });

            return clinicalTrials;
        }
    }
}
