using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Google.Apis.Auth.AspNetCore3;

using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

using Unitic_BE.Dtos;
using Unitic_BE.Services.Interfaces;
using Unitic_BE.Models;

namespace Unitic_BE.Controllers
{
    [Route("UniTic/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IGoogleService _googleService;
        
        public AuthController(IAuthService authService, IConfiguration config, IGoogleService googleService)
        {
            _authService = authService;
            _config = config;
            _googleService = googleService;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            try
            {
                //Console.WriteLine("Starting Google login process");

                // Configure the redirect URI to be the google-response endpoint
                var properties = new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(GoogleResponse), "Auth", null, Request.Scheme),                    
                    //Items =
                    //{
                    //    { "scheme", GoogleOpenIdConnectDefaults.AuthenticationScheme },
                    //    { "returnUrl", "/signin-google" } // Where to redirect after successful authentication]
                    //}
                };
                //foreach (var item in properties.Items)
                //Console.WriteLine($"Test gia tri url tra ve: {item} ");

                // Gọi đến google login.
                return Challenge(properties, GoogleOpenIdConnectDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initiating Google login: {ex.Message}");
                return StatusCode(500, $"Error initiating Google login: {ex.Message}");
            }
        }


        [HttpGet("google-response")]
        //[HttpGet("/signin-google")]
        public async Task<IActionResult> GoogleResponse()
        {
            //Console.WriteLine("Processing Google response");
            try
            {              
                if (!User.Identity.IsAuthenticated)
                {
                    //Console.WriteLine("User not authenticated in GoogleResponse");
                    return Unauthorized("Authentication failed");
                }
                // Thông tin người dùng
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //Console.WriteLine($"Google user authenticated successfully: {email}");

                var result = await HttpContext.AuthenticateAsync();
                // JWT token
                var idToken = result.Properties.GetTokenValue("id_token");
                // Token use to access googleApi
                var accessToken = result.Properties.GetTokenValue("access_token");
                // Refresh token again
                var refreshToken = result.Properties.GetTokenValue("refresh_token");
                return Ok(new { email, name, id, idToken, accessToken, refreshToken, message = "Authentication successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Authentication error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerUser)
        {
            if(registerUser == null)
                return BadRequest("Invalid user data");
            await _authService.Register(registerUser);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginUser)
        {
            var user = await _authService.GetUser(loginUser);
            //check có user
            if (user == null)
                return Unauthorized("Invalid username or password");
            //có user thì generate token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),// mã định danh token
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // định danh user
                new Claim(ClaimTypes.Name, loginUser.UserName)
             };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:exp"])), 
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 3. Trả về token
            return Ok(new LoginResponseDto
            {
                Token = tokenString,
                Expire = token.ValidTo
            });
        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logout Succesful");
        }
    }
}