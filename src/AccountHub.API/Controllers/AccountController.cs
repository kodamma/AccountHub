using AccountHub.API.Models;
using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountHub.API.Controllers
{
    [ApiController]
    [Route("account-hub/api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        public AccountController(IMapper mapper, IMediator mediator)
        {
            this.mapper = mapper;
            this.mediator = mediator;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromForm] SignUpAccountModel model)
        {
            var result = await mediator.Send(mapper.Map<AddAccountCommand>(model));
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }
    }
}
