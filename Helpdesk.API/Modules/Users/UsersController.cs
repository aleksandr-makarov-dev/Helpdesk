
using System.Net;
using Helpdesk.API.Models;
using Helpdesk.API.Modules.Users.Models;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.API.Modules.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController:ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
        {
            var registerResult = await _userService.RegisterUserAsync(request);

            if (registerResult.IsFailed)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "400 Bad Request",
                    Detail = registerResult.Errors.First().Message
                });
            }

            return Ok(new MessageResponse("You account is registered successfully!"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var loginResult = await _userService.LoginUserAsync(request);

            if (loginResult.IsFailed)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = "401 Unauthorized",
                    Detail = loginResult.Errors.First().Message
                });
            }

            return Ok(loginResult.Value);
        }
    }
}
