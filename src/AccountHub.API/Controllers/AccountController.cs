using AccountHub.API.Models;
using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccountHub.API.Controllers
{
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
    }
}
