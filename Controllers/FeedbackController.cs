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

    [HttpGet("{eventId}")]
    public async Task<ActionResult<List<Feedback>>> GetFeedbacksByEventId(string eventId)
    {
        var feedback = await _service.GetByEventIdAsync(eventId);
        if (feedback == null) return NotFound();
        return Ok(feedback);
    }
    [HttpGet("{feedbackId}")]
    public async Task<ActionResult<Feedback>> GetFeedbackById(string feedbackId)
    {
        var feedback = await _service.GetByIdAsync(feedbackId);
        if (feedback == null) return NotFound();
        return Ok(feedback);
    }

    [HttpPost]
    public async Task<ActionResult<Feedback>> CreateFeedback([FromBody] FeedbackRequest feedbackDto)
    {
        try
        {
            await _service.CreateAsync(feedbackDto);
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

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteFeedback(string id)
    // {
    //     await _service.DeleteAsync(id);
    //     return Ok();
    // }
}