using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telecom.UI.Models;
using Telecom.UI.Services;

namespace Telecom.UI.Controllers;

public class TicketController : Controller
{
    private readonly ApiService _apiService;

    public TicketController(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var tickets = await _apiService.GetTicketsAsync();
        return View(tickets);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var success = await _apiService.CreateTicketAsync(model);
        if (success)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to create ticket.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var ticket = await _apiService.GetTicketByIdAsync(id);
        if (ticket == null) return NotFound();
        return View(ticket);
    }

    [HttpGet]
    public IActionResult Resolve(Guid id)
    {
        ViewBag.TicketId = id;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Resolve(Guid ticketId, string resolutionDetail)
    {
        var success = await _apiService.ResolveTicketAsync(ticketId, resolutionDetail);
        if (success)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to resolve ticket.");
        ViewBag.TicketId = ticketId;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Assign(Guid id)
    {
        var technicians = await _apiService.GetTechniciansAsync();
        ViewBag.TicketId = id;
        return View(technicians);
    }

    [HttpPost]
    public async Task<IActionResult> Assign(Guid ticketId, Guid technicianId)
    {
        var success = await _apiService.AssignTechnicianAsync(ticketId, technicianId);
        if (success)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Failed to assign technician.");
        var technicians = await _apiService.GetTechniciansAsync();
        ViewBag.TicketId = ticketId;
        return View(technicians);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _apiService.DeleteTicketAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
