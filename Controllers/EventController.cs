using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Enums;
using Unitic_BE.Requests;

namespace Unitic_BE.Controllers
{
    [Route("Unitic/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _myEventService;
        public EventController(IEventService myEventService)
        {
            _myEventService = myEventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var myEvents = await _myEventService.GetAllEvents();
            return Ok(myEvents);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetPrivateEvents([FromQuery]EventStatus status)
        {
            var myEvents = await _myEventService.GetAllEventsByStatus(status);
            return Ok(myEvents);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(string id)
        {
            var myEvent = await _myEventService.GetEventByIdAsync(id);
            if (myEvent == null)
            {
                return NotFound("Event not found.");
            }
            return Ok(myEvent);
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] EventRequest myEvent)
        {
            if (myEvent == null)
            {
                return BadRequest("Invalid myEvent data.");
            }
            await _myEventService.AddEventAsync(myEvent);
            return Ok("Event added successfully");
        }

        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEvent(string eventId, [FromBody] EventRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid update data.");
            }
            await _myEventService.UpdateEventAsync(eventId, request);
            return Ok("Event updated successfully");
        }

        [HttpPut("status/{eventId}")]
        [Authorize(Roles = "Admin,Moderator,Organizer")]
        public async Task<IActionResult> UpdateEventStatus(string eventId, [FromQuery]EventStatus status)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (status != EventStatus.Completed && userRole == IdentityRoleConstants.Organizer)
            {
                return Unauthorized("Organizer can only update event status to Completed.");
            }
            await _myEventService.UpdateEventStatusAsync(eventId, status);
            return Ok("Event status updated successfully");
        }
    }
}
