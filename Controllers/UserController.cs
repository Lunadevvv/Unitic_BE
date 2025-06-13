using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitic_BE.Models;
using Unitic_BE.Services.Interfaces;

namespace Unitic_BE.Controllers
{
    [Route("UniTic/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Get/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("No user found");
            }
            return Ok(user);
        }

        [HttpPut("Update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
     
            if (user.UserId <= 0)
                return BadRequest("Invalid User ID");

            var existingUser = await _userService.GetUserRealByIdAsync(user.UserId);

            if (existingUser == null)
                return NotFound("User not found");

            // 3. Cập nhật từng trường (tránh lỗi tracking)
            existingUser.UserName = user.UserName;
            existingUser.Password = user.Password;
            existingUser.Hobby = user.Hobby;

            return Ok(existingUser);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("User deleted");
        }
    }
}
