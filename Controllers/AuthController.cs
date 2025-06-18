using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.Requests;
using Unitic_BE.Services;
using WebTicket.Domain.Requests;

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
            string acessToken = await _accountService.LoginAsync(loginRequest);

            return Ok(new LoginResponse
            {
                Message = "Login successful.",
                // Trả về token vào response body để tránh độ trễ lần đầu tạo cookie
                Token = acessToken
            });
        }

        [HttpPost("logout")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> LogoutAsync()
        {
            //thêm value rỗng vào ACCESS_TOKEN cookie để xóa token hiện tại
            HttpContext.Response.Cookies.Append("ACCESS_TOKEN",
                "");

            return Ok("Log out sucessful");
        }

    }

}

