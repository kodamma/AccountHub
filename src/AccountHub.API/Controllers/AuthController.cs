using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AccountHub.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccountHub.API.Controllers
{
    [ApiController]
    [Route("account-hub/api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IAuthenticationService authService;
        private readonly IConfiguration conf;
        public AuthController(IMediator mediator, IAuthenticationService authService, IConfiguration conf)
        {
            this.authService = authService;
            this.mediator = mediator;
            this.conf = conf;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginCommand command)
        {
            var result = await mediator.Send(command);
            if(result.IsSuccess)
            {
                HttpContext.User.Claims.Append(new Claim("Id", result.Value.AccountId));
                HttpContext.Response.Cookies
                    .Append("REFRESH_TOKEN", result.Value.RefreshToken, new CookieOptions()
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(int.Parse(conf["JWTOptions:RefreshLifetime"]!))
                    });
                return Ok(result.Value);
            }
            return Unauthorized(result.Errors);
        }
    }
}
