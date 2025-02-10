using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AccountHub.Application.CQRS.Commands.Authentication.Logout;
using AccountHub.Application.Options;
using AccountHub.Application.Responses;
using Kodamma.Common.Base.API;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

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
        public async Task<ActionResult<SignUpAccountResponse>> SignUp([FromForm] AddAccountCommand command)
        {
            var result = await Mediator.Send(command);
            return result.IsSuccess 
                ? Ok(result.Value) : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginCommand command)
        {
            var result = await Mediator.Send(command);
            if(result.IsFailure) 
                return BadRequest(result.Errors);

            CookieOptions cookieOptions = new CookieOptions()
            {
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(int.Parse(options.LifeTime.ToString()))
            };

            Response.Cookies.Append("KS_REF_TOKEN", result.Value.RefreshToken, cookieOptions);  
            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var accessToken = Request.Headers["Authorization"]!.ToString()
                .Substring("Bearer ".Length);
            var refToken = Request.Cookies["KS_REF_TOKEN"];

            var result = await Mediator.Send(new LogoutCommand()
            {
                AccountId = UserId,
                AccessToken = accessToken,
                RefreshToken = refToken ?? string.Empty,
            });

            if (result.IsFailure) return BadRequest(result.Errors);

            return Ok();
        }
    }
}
