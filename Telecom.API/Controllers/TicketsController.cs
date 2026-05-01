using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telecom.Business.DTOs.Ticket;
using Telecom.Business.Services.Interfaces;

namespace Telecom.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    // Sadece "Agent" rolündekiler yeni bilet açabilir.
    [Authorize(Roles = "Agent")]
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ticket = await _ticketService.CreateTicketAsync(dto, currentUserId);
        return Ok(ticket);
    }

    // Herhangi bir giriş yapmış kullanıcı tüm biletleri görebilir.
    [HttpGet]
    public async Task<IActionResult> GetAllTickets()
    {
        var tickets = await _ticketService.GetAllTicketsAsync();
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    // Sadece "Admin" veya "Agent" rolündekiler biletlere teknisyen atayabilir.
    [Authorize(Roles = "Admin,Agent")]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignTechnician([FromBody] AssignTechnicianDto dto)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _ticketService.AssignTechnicianAsync(dto, currentUserId);
        if (!result) return BadRequest("Assignment failed. Ticket might not exist.");
        return Ok(new { Message = "Technician assigned successfully." });
    }

    // "Admin" veya saha personeli olan "Technician" statüyü değiştirebilir.
    [Authorize(Roles = "Admin,Technician")]
    [HttpPut("status")]
    public async Task<IActionResult> ChangeStatus([FromBody] ChangeTicketStatusDto dto)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _ticketService.ChangeStatusAsync(dto, currentUserId);
        if (!result) return BadRequest("Status change failed. Ticket might not exist.");
        return Ok(new { Message = "Status changed successfully." });
    }
}
