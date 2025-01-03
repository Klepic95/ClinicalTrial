using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.Business.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalRecordController : ControllerBase
    {
        private readonly IClinicalTrialService _clinicalTrialService;
        private readonly ILogger<ClinicalRecordController> _logger;

        public ClinicalRecordController(IClinicalTrialService clinicalTrialService, ILogger<ClinicalRecordController> logger)
        {
            _clinicalTrialService = clinicalTrialService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicalRecordDTO>> GetById(Guid id)
        {
            try
            {
                var trial = await _clinicalTrialService.GetClinicalTrialByIdAsync(id);
                if (trial == null)
                {
                    _logger.LogWarning("Clinical record not found for ID: {Id}", id);
                    return NotFound("Clinical record not found.");
                }
                return Ok(trial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching clinical record for ID: {Id}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicalRecordDTO>>> FilterClinicalTrials([FromQuery]string? status, [FromQuery]int? minParticipants, [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate)
        {
            try
            {
                var trials = await _clinicalTrialService.GetFilteredTrialsAsync(status, minParticipants, startDate, endDate);
                if (!trials.Any())
                {
                    _logger.LogWarning("No trials found matching the given criteria.");
                    return NotFound("No trials found matching the given criteria.");
                }

                return Ok(trials);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occured. Please check exception for more details.");
                return BadRequest(ex.Message);
            }
        }
    }
}
