using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AccountHub.Application.CQRS.Commands.Authentication.Logout;
using AccountHub.Application.Options;
using AccountHub.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Kodamma.Common.Base.API;

namespace AccountHub.API.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromForm]LoginCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsFailure)
                return Unauthorized(result.Errors);
            User.Claims.Append(new Claim(ClaimTypes.NameIdentifier, result.Value.AccountId.ToString()));
            SetRefreshToken(result.Value.RefreshToken);
            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await Mediator.Send(new LogoutCommand()
            {
                AccountId = UserId,
                AccessToken = GetTokenFromHeader(),
                RefreshToken = Request.Cookies["KS_REFRESH_TOKEN"] ?? string.Empty,
            });
            return result.IsSuccess ? Ok() : Unauthorized();
        }

        private string GetTokenFromHeader()
            => HttpContext.Request.Headers["Authorization"]!.ToString()
                .Substring("Bearer ".Length);

        private void SetRefreshToken(string token)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(10)
            };
            Response.Cookies.Append("KS_REFRESH_TOKEN", token, cookieOptions);
        }
    }
}
