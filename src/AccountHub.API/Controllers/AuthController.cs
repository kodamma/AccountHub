using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AccountHub.Application.Options;
using AccountHub.Application.Responses;
using Kodamma.Common.Base.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AccountHub.API.Controllers
{
    [Route("account-hub/api/v1/auth")]
    public class AuthController : BaseApiController
    {
        private readonly JwtOptions options;
        public AuthController(IOptions<JwtOptions> options)
        {
            this.options = options.Value;
        }

        [AllowAnonymous]
        [HttpPost("sign-up")]
        public async Task<ActionResult<SignUpAccountResponse>> SignUp(
            [FromForm] AddAccountCommand command)
        {
            var result = await Mediator.Send(command);
            return result.IsSuccess
                ? Ok(result.Value) : BadRequest(result.Errors);
        }

        
        

        //[HttpPost("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    var accessToken = Request.Headers["Authorization"]!.ToString()
        //        .Substring("Bearer ".Length);
        //    var refToken = Request.Cookies["KS_REF_TOKEN"];

        //    var result = await Mediator.Send(new LogoutCommand()
        //    {
        //        AccountId = UserId,
        //        AccessToken = accessToken,
        //        RefreshToken = refToken ?? string.Empty,
        //    });

        //    if (result.IsFailure) return BadRequest(result.Errors);

        //    return Ok();
        //}
    }
}
