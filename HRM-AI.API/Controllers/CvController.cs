using Google;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using HRM_AI.Services.Models.CampaignModels;
using HRM_AI.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_AI.API.Controllers
{
    [Route("api/cv")]
    [ApiController]
    public class CvController : ControllerBase
    {
        private readonly ResumeParserAIService _parser;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly ICVParseService _cVParserService;
        private readonly IOpenAiService _openAiService;

        public CvController(ResumeParserAIService parser, IGoogleDriveService googleDriveService, ICVParseService cVParserService, IOpenAiService openAiService)
        {
            _parser = parser;
            _googleDriveService = googleDriveService;
            _cVParserService = cVParserService;
            _openAiService = openAiService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest(new ResponseModel
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "Missing file"
                });
            }

            try
            {
                var parsedJson = await _parser.ParseAsync(file);

                var model = new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "File parsed successfully",
                    Data = parsedJson
                };

                return StatusCode(model.Code, model);
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new ResponseModel
                {
                    Code = StatusCodes.Status502BadGateway,
                    Message = "Error calling Resume Parser API: " + httpEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = "Internal error: " + ex.Message
                });
            }
        }
        //[HttpPost("uploads")]
        //public async Task<IActionResult> UploadFile(IFormFile file)
        //{
        //    try
        //    {
        //        var model = new ResponseModel();
        //        if (file != null)
        //        {
        //            using var stream = file.OpenReadStream();
        //            var fileId = await _googleDriveService.UploadFileAsync(stream, file.FileName, file.ContentType);

        //            model = new ResponseModel
        //            {
        //                Code = StatusCodes.Status200OK,
        //                Message = "File parsed successfully",
        //                Data = fileId
        //            };

        //            return StatusCode(model.Code, model);
        //        }

        //        model = new ResponseModel
        //        {
        //            Code = StatusCodes.Status400BadRequest,
        //            Message = "File parsed fail",
        //        };

        //        return StatusCode(model.Code, model);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}


        [HttpPost("parse")]
            public async Task<IActionResult> ParseCV(IFormFile file, Guid campaignPositionId)
            {
                try
                {
                    if (file == null || file.Length == 0)
                        return BadRequest(new ResponseModel { Code = 400, Message = "File không hợp lệ." });

                    var result = await _openAiService.ParseCVAsync(file, campaignPositionId);
                    return StatusCode(result.Code, result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new ResponseModel
                    {
                        Code = 500,
                        Message = ex.Message
                    });
                }
            }
        

    }
}
