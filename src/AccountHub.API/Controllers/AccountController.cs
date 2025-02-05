using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.CQRS.Commands.Account.ConfirmEmail;
using AutoMapper;
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
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly IConfiguration conf;
        public AccountController(IMapper mapper, IMediator mediator, IConfiguration conf)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.conf = conf;
        }

        [AllowAnonymous]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromForm]AddAccountCommand command)
        {
            var result = await mediator.Send(command);
            if(result.IsSuccess)
            {

                return Ok(result.Value);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("confirm-email/{id:guid}")]
        public async Task<IActionResult> ConfirmEmail(Guid id)
        {
            var result = await mediator.Send(new ConfirmEmailCommand() { AccountId = id});
            return Ok();
        }
    }
}
