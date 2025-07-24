using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.DTOs.Responses;
using Unitic_BE.Entities;

namespace Unitic_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IUniversityService _universityService;

        public ProfileController(IProfileService profileService, IUniversityService universityService)
        {
            _profileService = profileService;
            _universityService = universityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUserInformation()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Invalid User!");

            var user = await _profileService.GetCurrentUserInformation(userId);
            return Ok(new ProfileResponse
            {
                Id = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Mssv = user.Mssv,
                Wallet = user.wallet,
                University = await _universityService.GetUniversityById(user.UniversityId),
                Role = user.Role
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateUserInformation request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Invalid User!");

            await _profileService.UpdateUserInformation(userId, request);
            return Ok("Profile updated successfully!");
        }
    }
}