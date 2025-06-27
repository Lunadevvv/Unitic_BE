using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Requests;
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

        [HttpGet("private")]
        public async Task<IActionResult> GetPrivateEvents()
        {
            var myEvents = await _myEventService.GetAllPrivateEvents();
            return Ok(myEvents);
        }

        [HttpGet("sold-out")]
        public async Task<IActionResult> GetSoldOutEvents()
        {
            var myEvents = await _myEventService.GetAllSoldOutEvents();
            return Ok(myEvents);
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedEvents()
        {
            var myEvents = await _myEventService.GetAllPublishedEvents();
            return Ok(myEvents);
        }

        [HttpGet("Cancelled")]
        public async Task<IActionResult> GetCancelledEvents()
        {
            var myEvents = await _myEventService.GetAllCancelledEvents();
            return Ok(myEvents);
        }

        [HttpGet("Completed")]
        public async Task<IActionResult> GetCompletedEvents()
        {
            var myEvents = await _myEventService.GetAllCompletedEvents();
            return Ok(myEvents);
        }
        [HttpGet("InProgress")]
        public async Task<IActionResult> GetInProgressEvents()
        {
            var myEvents = await _myEventService.GetAllInProgressEvents();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(string id, [FromBody] EventRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid update data.");
            }
            await _myEventService.UpdateEventAsync(id, request);
            return Ok("Event updated successfully");
        }

        [HttpPut("status/{id}")]
        [Authorize(Roles = "Admin,Moderator,Organizer")]
        public async Task<IActionResult> UpdateEventStatus(string id, EventUpdateStatusRequest eventUpdateStatusRequest)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (eventUpdateStatusRequest.status != EventStatusConstant.Completed && userRole == IdentityRoleConstants.Organizer)
            {
                return Unauthorized("Organizer can only update event status to Completed.");
            }
            await _myEventService.UpdateEventStatusAsync(id, eventUpdateStatusRequest);
            return Ok("Event status updated successfully");
        }
    }
}
