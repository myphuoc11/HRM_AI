using Google;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
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
        private readonly GoogleDriveService _gds;
        private readonly ICVParseService _cVParserService;

        public CvController(ResumeParserAIService parser, GoogleDriveService gds, ICVParseService cVParserService)
        {
            _parser = parser;
            _gds = gds;
            _cVParserService = cVParserService;
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
        [HttpPost("uploads")]
        public async Task<IActionResult> Uploads(IFormFile file)
        {
            var res = new ResponseModel();

            if (file == null || file.Length == 0)
            {
                res.Code = StatusCodes.Status400BadRequest;
                res.Message = "Missing or empty file";
                res.Data = null;
                return StatusCode(res.Code, res);
            }

            try
            {
                string link = await _gds.UploadFileAsync(
                    file.OpenReadStream(),
                    file.FileName,
                    file.ContentType);

                res.Code = StatusCodes.Status200OK;
                res.Message = "File uploaded successfully";
                res.Data = new { link };
                return StatusCode(res.Code, res);
            }
            catch (GoogleApiException gex)
            {
                res.Code = StatusCodes.Status502BadGateway;
                res.Message = "Error connecting to Google Drive";
                res.Data = new { error = gex.Message };
                return StatusCode(res.Code, res);
            }
            catch (Exception ex)
            {
                res.Code = StatusCodes.Status500InternalServerError;
                res.Message = "Internal server error";
                res.Data = new { error = ex.Message };
                return StatusCode(res.Code, res);
            }
        }
      

            [HttpPost("parse")]
            public async Task<IActionResult> ParseCV(IFormFile file)
            {
                try
                {
                    if (file == null || file.Length == 0)
                        return BadRequest(new ResponseModel { Code = 400, Message = "File không hợp lệ." });

                    var result = await _cVParserService.ParseCVAsync(file);
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
