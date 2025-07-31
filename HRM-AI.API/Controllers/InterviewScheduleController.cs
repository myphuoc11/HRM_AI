using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models.CampaignModels;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.InterviewScheduleModels;
using HRM_AI.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_AI.API.Controllers
{
    [Route("api/interview-schedules")]
    [ApiController]
    public class InterviewScheduleController : ControllerBase
    {
        private readonly IInterviewScheduleService _interviewScheduleService;

        public InterviewScheduleController(IInterviewScheduleService interviewScheduleService)
        {
            _interviewScheduleService = interviewScheduleService;
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] InterviewScheduleAddModel interviewScheduleAddModel)
        {

            try
            {
                var result = await _interviewScheduleService.Add(interviewScheduleAddModel);
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
