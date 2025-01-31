using AccountHub.Application.CQRS.Commands.Authentication.Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountHub.API.Controllers
{
    [ApiController]
    [Route("account-hub/api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMediator mediator;
        public AuthController(IHttpContextAccessor contextAccessor, IMediator mediator)
        {
            this.contextAccessor = contextAccessor;
            this.mediator = mediator;
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var accountId = contextAccessor!.HttpContext!.User.Claims.FirstOrDefault(x
                => x.Type == "Id")!.Value;
            var header = contextAccessor.HttpContext.Request.Headers["Authorization"].SingleOrDefault();
            if(header == null || !header.StartsWith("Bearer "))
                return Unauthorized("JWT token is missing or invalid.");

            var token = header.Substring("Bearer ".Length);
            var result = await mediator.Send(new LogoutCommand()
            {
                AccountId = Guid.Parse(accountId),
                JwtToken = token,
            });
            return result.IsSuccess ? Ok() : BadRequest(result.Errors);
        }
    }
}
