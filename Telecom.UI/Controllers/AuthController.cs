using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telecom.UI.Models;
using Telecom.UI.Services;

namespace Telecom.UI.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _apiService;

    public AuthController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var token = await _apiService.LoginAsync(model);
        if (!string.IsNullOrEmpty(token))
        {
            // Set token in cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(120),
                Secure = true
            };
            Response.Cookies.Append("AuthToken", token, cookieOptions);
            return RedirectToAction("Index", "Ticket");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }
    
    [HttpGet]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        return RedirectToAction("Login");
    }
}
