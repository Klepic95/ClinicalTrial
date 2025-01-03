using MediatR;
using Microsoft.AspNetCore.Http;

namespace ClinicalTrial.Application.CQRS.Commands.ClinicalTrial
{
    public sealed record CreateClinicalTrialCommand(IFormFile file) : IRequest<Guid>
    {
    }
}
