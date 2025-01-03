using MediatR;

namespace ClinicalTrial.Application.CQRS.Queries
{
    public sealed record GetClinicalTrialByIdQuery(Guid Id) : IRequest<Domain.Entities.ClinicalTrial>
    {
    }
}
