using MediatR;

namespace ClinicalTrial.Application.CQRS.Queries
{
    public sealed record GetFilteredClinicalTrialsQuery(string? status, int? minParticipants, DateTime? startDate, DateTime? endDate) : IRequest<IEnumerable<Domain.Entities.ClinicalTrial>>
    {
    }
}
