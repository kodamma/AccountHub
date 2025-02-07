using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AccountHub.Application.Options;
using AccountHub.Application.Responses;
using MassTransit.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AccountHub.API.Controllers
{
    [ApiController]
    [Route("account-hub/api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly JwtOptions options;
        public AuthController(IMediator mediator, IOptions<JwtOptions> options)
        {
            this.mediator = mediator;
            this.options = options.Value;
        }

        [AllowAnonymous]
        [HttpPost("sign-up")]
        public async Task<ActionResult<SignUpAccountResponse>> SignUp([FromForm] AddAccountCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginCommand command)
        {
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                HttpContext.User.Claims.Append(new Claim("Id", result.Value.AccountId));
                HttpContext.Response.Cookies
                    .Append("REFRESH_TOKEN", result.Value.RefreshToken, new CookieOptions()
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(int.Parse(options.LifeTime.ToString()))
                    });
                return Ok(result.Value);
            }
            return Unauthorized(result.Errors);
        }
    }
}
