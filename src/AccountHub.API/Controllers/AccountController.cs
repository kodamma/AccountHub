using AccountHub.Application.CQRS.Commands.Account.SendEmailConfirmation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountHub.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("account-hub/api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator mediator;
        public AccountController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("send-email-confirmationa")]
        public async Task<IActionResult> SendEmailConfirmation()
        {
            if(Guid.TryParse(HttpContext.User.Claims.FirstOrDefault(x
                => x.Type == "Id")?.Value, out Guid accountId))
            {
                var result = await mediator.Send(new SendEmailConfirmationCommand()
                {
                    AccountId = accountId
                });
            }
            return BadRequest();
        }

        [HttpGet("сonfirm-emai/{email}")]
        public async Task<IActionResult> ConfirmEmail(string email)
        {
            return Ok();
        }
    }
}
