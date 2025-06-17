using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.Requests;
using Unitic_BE.Services;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService service)
        {
            // Constructor logic if needed
            _accountService = service;

        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null)
            {
                return BadRequest("Invalid registration request.");
            }
            await _accountService.RegisterAsync(registerRequest);
            return Ok("Registration successful.");
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest("Invalid login request.");
            }
            await _accountService.LoginAsync(loginRequest);
            return Ok(HttpContext.Request.Headers["Authorization"]);
        }
        [HttpPost("logout")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> LogoutAsync()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token.");
            }
            await _accountService.LogoutAsync(token);
            return Ok("Logout successful.");
        }

    }

}

