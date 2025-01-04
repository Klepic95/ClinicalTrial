using ClinicalTrial.Application.CQRS.Commands.ClinicalTrial;
using ClinicalTrial.Application.CQRS.Queries;
using ClinicalTrial.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrial.Presentation.Controllers
{
    [Route("clinicalTrial")]
    public class ClinicalTrialController : ControllerBase
    {
        private readonly ISender _sender;

        public ClinicalTrialController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0 || Path.GetExtension(file.FileName) != ".json")
            {
                return BadRequest("Invalid file type.");
            }

            var command = new CreateClinicalTrialCommand(file);
            var result = await _sender.Send(command);

            try
            {
                return Ok($"File processed successfully. Id of the processed file is: {result}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetApiExceptionMessage());
            }
        }

        [HttpGet("get/{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var query = new GetClinicalTrialByIdQuery(id);
                var trial = await _sender.Send(query);
                if (trial == null)
                {
                    return NotFound("Clinical record not found.");
                }
                return Ok(trial);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetFilterClinicalTrials([FromQuery] string? status, [FromQuery] int? minParticipants, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var query = new GetFilteredClinicalTrialsQuery(status, minParticipants, startDate, endDate);
                var trials = await _sender.Send(query);
                if (!trials.Any())
                {
                    return NotFound("No trials found matching the given criteria.");
                }

                return Ok(trials);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
