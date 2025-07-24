using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models.CampaignModels;
using HRM_AI.Services.Models;
using HRM_AI.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HRM_AI.Services.Models.CampaignPositionModels;

namespace HRM_AI.API.Controllers
{
    [Route("api/campaign-positions")]
    [ApiController]
    public class CampaignPositionController : ControllerBase
    {
        private readonly ICampaignPositionService _campaignPositionService;
        public CampaignPositionController(ICampaignPositionService campaignPositionService)
        {
            _campaignPositionService = campaignPositionService;
        }
        [HttpPost]

        public async Task<IActionResult> CreateCampaignPosition([FromBody] CampaignPositionAddModel campaignPositionAddModel)
        {
            try
            {
                var result = await _campaignPositionService.Add(campaignPositionAddModel);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampaignPosition(Guid id, [FromBody] CampaignPositionUpdateModel campaignPositionUpdateModel)
        {
            try
            {
                var result = await _campaignPositionService.Update(campaignPositionUpdateModel, id);
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaignPosition(Guid id)
        {
            try
            {
                var result = await _campaignPositionService.Delete(id);
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
