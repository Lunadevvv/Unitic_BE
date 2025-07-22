using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;
using Unitic_BE.Exceptions;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _service;

    public FeedbackController(IFeedbackService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Feedback>>> GetAllFeedback()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Feedback>> GetFeedbackById(string id)
    {
        var feedback = await _service.GetByIdAsync(id);
        if (feedback == null) return NotFound();
        return Ok(feedback);
    }

    [HttpPost]
    public async Task<ActionResult<Feedback>> CreateFeedback([FromBody] CreateFeedback feedbackDto)
    {
        try
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new NotValidUserException();
            }
            Feedback feedback = new Feedback
            {                
                BookingId = feedbackDto.BookingId,
                Content = feedbackDto.Review,
                UserId = userId,
            };
            await _service.CreateAsync(feedback, feedbackDto.EventId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFeedback(string id, [FromBody] Feedback feedback)
    {
        if (id != feedback.FeedbackId) return BadRequest();
        await _service.UpdateAsync(feedback);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFeedback(string id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
}