using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models.AccountModels;
using HRM_AI.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_AI.API.Controllers
{
    [Route("api/v1/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthenticationController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] AccountSignUpModel accountSignUpModel)
        {
            try
            {
                var result = await _accountService.SignUp(accountSignUpModel);
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

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] AccountSignInModel accountSignInModel)
        {
            try
            {
                var result = await _accountService.SignIn(accountSignInModel);
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
