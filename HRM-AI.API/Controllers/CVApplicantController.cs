using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models.CampaignModels;
using HRM_AI.Services.Models;
using HRM_AI.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HRM_AI.Services.Models.CVApplicantModels;

namespace HRM_AI.API.Controllers
{
    [Route("api/cvApplicants")]
    [ApiController]
    public class CVApplicantController : ControllerBase
    {
        private readonly ICVApplicantService _cVApplicantService;

        public CVApplicantController(ICVApplicantService cVApplicantService)
        {
            _cVApplicantService = cVApplicantService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCVApplicant([FromForm] CVApplicantAddModel cVApplicantAddModel)
        {
            try
            {
                var result = await _cVApplicantService.Add(cVApplicantAddModel);
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
