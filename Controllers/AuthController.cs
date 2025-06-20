using System.Security.Claims;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.Contracts;
using Unitic_BE.Requests;
using Unitic_BE.Services;
using Unitic_BE.Requests;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IGoogleService _googleService;
        private readonly IEmailService _emailService;

        public AuthController(IAccountService service, IGoogleService googleService, IEmailService emailService)
        {
            // Constructor logic if needed
            _accountService = service;
            _googleService = googleService;
            _emailService = emailService;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            try
            {
                // Redirect to Google authentication
                var redirectUrl = Url.Action(nameof(GoogleResponse), "Auth", null, Request.Scheme);
                var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
                return Challenge(properties, GoogleOpenIdConnectDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                // Get the Google user information from the authentication result
                var authenticateResult = await HttpContext.AuthenticateAsync(GoogleOpenIdConnectDefaults.AuthenticationScheme);
                if (!authenticateResult.Succeeded)
                {
                    return Unauthorized();
                }

                string token = await _googleService.LoginWithGoogleAsync(authenticateResult.Principal);

                return Ok(new { Message = "Login successful.", Token = token });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
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
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("ACCESS_TOKEN"); // Xóa cookie đăng nhập

            return Ok("Log out successful");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            var token = await _accountService.ForgotPassword(forgotPasswordRequest.Email);

            var body = $"Testing {token}";
            var sendEmailRequest = new SendEmailRequest(forgotPasswordRequest.Email, "Reset Password", body);
            await _emailService.SendEmailAsync(sendEmailRequest);
            return Ok("Please check your email!");
        }

        [HttpPost("reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _accountService.ResetPassword(userId, resetPasswordRequest.NewPassword);

            Response.Cookies.Delete("RESET_TOKEN");
            return Ok("Reset password successfully!");
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Invalid User!");

            await _accountService.ChangePassword(userId, changePasswordRequest);
            return Ok("Changed Password successfully!");
        }
    }

}

