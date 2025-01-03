using MediatR;

namespace ClinicalTrial.Application.CQRS.Commands.ClinicalTrial
{
    public sealed record DeleteClinicalTrialCommand(Domain.Entities.ClinicalTrial ClinicalTrial) : IRequest 
    {
    }
}
