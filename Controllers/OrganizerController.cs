using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Unitic_BE.Abstracts;

namespace Unitic_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizerController : ControllerBase
    {
        private readonly IOrganizerService _organizerService;

        public OrganizerController(IOrganizerService organizerService)
        {
            _organizerService = organizerService;
        }

        [HttpPost("assign/{eventId}/{userId}")]
        public async Task<IActionResult> AssignOrganizerToEvent(string eventId, string userId)
        {
            if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(userId))
            {
                return BadRequest("Event ID and User ID cannot be null or empty.");
            }
            var result = await _organizerService.AssignOrganizerToEvent(userId, eventId);
            if (result)
            {
                return Ok("Organizer assigned successfully.");
            }
            return BadRequest("Failed to assign organizer.");
        }

        [HttpDelete("remove/{organizerId}")]
        public async Task<IActionResult> RemoveOrganizerFromEvent(string organizerId)
        {
            var result = await _organizerService.RemoveOrganizerFromEvent(organizerId);
            if (result)
            {
                return Ok("Organizer removed successfully.");
            }
            return NotFound("Organizer not found.");
        }

        [HttpGet("events/{userId}")]
        public async Task<IActionResult> GetEventsByOrganizer(string userId)
        {
            var events = await _organizerService.GetEventsByOrganizer(userId);
            if (events == null || !events.Any())
            {
                return NotFound("No events found for this organizer.");
            }
            return Ok(events);
        }
    }
}