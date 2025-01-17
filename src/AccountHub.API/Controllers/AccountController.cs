using AccountHub.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountHub.API.Controllers
{
    [ApiController]
    [Route("account-hub/api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromForm] SignUpAccountModel model)
        {
            return Ok();
        }
    }
}
