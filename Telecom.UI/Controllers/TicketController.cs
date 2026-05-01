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
        // Simple check to ensure user has token, otherwise redirect to login
        if (string.IsNullOrEmpty(Request.Cookies["AuthToken"]))
        {
            return RedirectToAction("Login", "Auth");
        }

        var tickets = await _apiService.GetTicketsAsync();
        return View(tickets);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (string.IsNullOrEmpty(Request.Cookies["AuthToken"]))
        {
            return RedirectToAction("Login", "Auth");
        }
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
}
