using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using Unitic_BE.Dtos;
using Unitic_BE.Services.Interfaces;

namespace Unitic_BE.Controllers
{
    [Route("UniTic/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
    
        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
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
    }
}