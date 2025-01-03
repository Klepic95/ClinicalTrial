using MediatR;

namespace ClinicalTrial.Application.CQRS.Commands.ClinicalTrial
{
    public sealed record UpdateClinicalTrialCommand(Domain.Entities.ClinicalTrial ClinicalTrial) : IRequest<Domain.Entities.ClinicalTrial>
    {
    }
}
