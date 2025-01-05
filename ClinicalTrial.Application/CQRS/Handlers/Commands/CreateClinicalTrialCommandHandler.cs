using ClinicalTrial.Application.CQRS.Commands.ClinicalTrial;
using ClinicalTrial.Domain.Abstractions;
using ClinicalTrial.Domain.Entities;
using ClinicalTrial.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Reflection;
using System.Security.AccessControl;

namespace ClinicalTrial.Application.CQRS.Handlers.Commands
{
    public class CreateClinicalTrialCommandHandler : IRequestHandler<CreateClinicalTrialCommand, Guid>
    {

        private readonly IRepository<Domain.Entities.ClinicalTrial> _clinicalTrialRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateClinicalTrialCommandHandler> _logger;
        private readonly string _schemaFilePath = Assembly.GetExecutingAssembly().GetManifestResourceNames()[0];

        public CreateClinicalTrialCommandHandler(IRepository<Domain.Entities.ClinicalTrial> clinicalTrialRepository, IUnitOfWork unitOfWork, ILogger<CreateClinicalTrialCommandHandler> logger)
        {
            _clinicalTrialRepository = clinicalTrialRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateClinicalTrialCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using var reader = new StreamReader(request.file.OpenReadStream());
                var jsonContent = await reader.ReadToEndAsync();

                // JSON will first be validated against a schema before proceeding
                if (!ValidateJson(jsonContent))
                {
                    _logger.LogError("{@SystemLog}", new SystemLog()
                    {
                        Event = Event.Create,
                        Comment = "Clinical Trial file schema validation",
                        CreatedAt = DateTime.UtcNow,
                        ChangeSet = new[] { request.file }
                    });
                    throw new Exception("Invalid JSON schema.");
                }

                var clinicalTrial = JsonConvert.DeserializeObject<Domain.Entities.ClinicalTrial>(jsonContent);

                if (clinicalTrial?.Participants < 1)
                {
                    _logger.LogError("{@SystemLog}", new SystemLog()
                    {
                        Event = Event.Create,
                        Comment = "Clinical Trial participants validation",
                        CreatedAt = DateTime.UtcNow,
                        ChangeSet = new[] { clinicalTrial }
                    });
                    throw new Exception("Participant number must be greater than 0.");
                }

                if (clinicalTrial.Status == "Ongoing" && clinicalTrial.EndDate == default)
                {
                    clinicalTrial.EndDate = clinicalTrial.StartDate.AddMonths(1);
                }
                if (clinicalTrial.EndDate != default)
                {
                    clinicalTrial.DurationInDays = (clinicalTrial.EndDate - clinicalTrial.StartDate).Days;
                }

                var generatedClinicalTrialId = await _clinicalTrialRepository.CreateAsync(clinicalTrial);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("{@SystemLog}", new SystemLog()
                {
                    Event = Event.Create,
                    Comment = "Clinical Trial record upload",
                    CreatedAt = DateTime.UtcNow,
                    ChangeSet = new[] { clinicalTrial }
                });

                return generatedClinicalTrialId;
            }
            catch (Exception ex)
            {
                _logger.LogError("{@SystemLog}", new SystemLog()
                {
                    Event = Event.Create,
                    Comment = "Clinical Trial upload general exception",
                    CreatedAt = DateTime.UtcNow,
                    ChangeSet = new[] { request.file }
                });
                throw;
            }
        }

        private bool ValidateJson(string jsonContent)
        {
            try
            {
                var schemaJson = GetSchemaJson();
                var schema = JSchema.Parse(schemaJson);

                var json = JObject.Parse(jsonContent);

                var isValid = json.IsValid(schema, out IList<string> errorMessages);

                if (!isValid)
                {
                    foreach (var errorMessage in errorMessages)
                    {
                        _logger.LogError("{@SystemLog}", new SystemLog()
                        {
                            Event = Event.Create,
                            Comment = "Clinical Trial JSON schema validation: " + errorMessage,
                            CreatedAt = DateTime.UtcNow,
                            ChangeSet = new[] { errorMessage }
                        });
                    }
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError("{@SystemLog}", new SystemLog()
                {
                    Event = Event.Create,
                    Comment = "Clinical Trial JSON schema validation",
                    CreatedAt = DateTime.UtcNow,
                    ChangeSet = new[] { ex }
                });
                return false;
            }
        }

        private string GetSchemaJson()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(_schemaFilePath))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
