using AccountHub.API.Models;
using AccountHub.Application.CQRS.Commands.Account.AddAccount;
using AccountHub.Application.CQRS.Commands.Authentication.Login;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountHub.API.Controllers
{
    [ApiController]
    [Route("account-hub/api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly IHttpContextAccessor httpContextAccessor;
        public AccountController(IMapper mapper, IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            this.mapper = mapper;
            this.mediator = mediator;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromForm] SignUpAccountModel model)
        {
            var result = await mediator.Send(mapper.Map<AddAccountCommand>(model));
            if(result.IsSuccess)
            {
                httpContextAccessor?.HttpContext!.Response.Cookies
                    .Append("RefreshToken", result.Value.RefreshToken);
                return Ok(result.Value);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromForm]SignInAccountModel model)
        {
            var result = await mediator.Send(mapper.Map<LoginCommand>(model));
            if (result.IsSuccess)
            {
                httpContextAccessor?.HttpContext!.Response.Cookies
                    .Append("RefreshToken", result.Value.RefreshToken);
                return Ok(result.Value);
            }
            return BadRequest(result.Errors);
        }
    }
}
