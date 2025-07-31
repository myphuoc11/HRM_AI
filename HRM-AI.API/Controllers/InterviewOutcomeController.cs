using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models.CampaignModels;
using HRM_AI.Services.Models;
using HRM_AI.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HRM_AI.Services.Models.InterviewOutcomeAddModels;

namespace HRM_AI.API.Controllers
{
    [Route("api/interview-outcomes")]
    [ApiController]
    public class InterviewOutcomeController : ControllerBase
    {
        private readonly IInterviewOutcomeService _interviewOutcomeService;
        public InterviewOutcomeController(IInterviewOutcomeService interviewOutcomeService)
        {
            _interviewOutcomeService = interviewOutcomeService;
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] InterviewOutcomeAddModel interviewOutcomeAddModel)
        {
            try
            {
                var result = await _interviewOutcomeService.Add(interviewOutcomeAddModel);
                return StatusCode(result.Code, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = ex.Message
                });
            }
        }
    }
}
