using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unitic_BE.Abstracts;
using Unitic_BE.Contracts;
using Unitic_BE.DTOs;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.Exceptions;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IEventService _eventService;
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;
    public BookingController(IBookingService bookingService, IEventService eventService, IAccountService accountService, IEmailService emailService)
    {
        _bookingService = bookingService;
        _eventService = eventService;
        _accountService = accountService;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var listBooking = await _bookingService.GetAllAsync();
        return Ok(listBooking);
    }

    [HttpGet("All/{userId}")]
    public async Task<IActionResult> GetAllUser(string userId)
    {
        var listBooking = await _bookingService.GetAllUserBooking(userId);
        return Ok(listBooking);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking == null) return NotFound();
        return Ok(booking);
    }


    [HttpPost("buy-ticket")]
    public async Task<IActionResult> BuyTicket([FromBody] BookingRequest bookingRequest)
    {
        try
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return BadRequest("User ID not found");
            var user = await _accountService.GetCurrentUserAsync(userId);
            var eventDetails = await _eventService.GetEventByIdAsync(bookingRequest.EventID);
            if (eventDetails == null)
                return NotFound("Event not found");
            await _bookingService.BuyTicketAsync(bookingRequest, userId);
            var body = $"Congratulation! Here is your ticket for the event: {bookingRequest.EventID}. " +
                       $"You have successfully purchased a ticket for the event. " +
                       $"Event Name: {eventDetails.Name}, " +
                       $"Event Date: {eventDetails.Date_Start}, " +
                       $"Thank you for your purchase!";
            var sendEmailRequest = new SendEmailRequest(user.Email, "Ticket bought successfully", body);
            await _emailService.SendEmailAsync(sendEmailRequest);
            return Ok("Ticket purchased successfully");
        }
        catch (ObjectNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (EventCancelException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (EventFinishException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (EventSoldOutException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest("Something went wrong: " + ex.Message);
        }
    }

    [HttpPut("transfer-ticket")]
    public async Task<IActionResult> TransferTicket([FromBody] TransferBookingRequest bookingRequest)
    {
        try
        {
            await _bookingService.TransferTicketAsync(bookingRequest);
            return Ok("Transfer ticket succesful");
        }
        catch (NotValidUserException nvu)
        {
            return BadRequest(nvu.Message + "in booking");
        }
    }
}