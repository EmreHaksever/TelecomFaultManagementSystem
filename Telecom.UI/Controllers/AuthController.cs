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
            // Token'ı HttpOnly cookie'ye yaz
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(120),
                Secure = false // local dev
            };
            Response.Cookies.Append("AuthToken", token, cookieOptions);

            // JWT payload'ı kütüphane olmadan elle çöz (Base64 decode)
            var parts = token.Split('.');
            if (parts.Length == 3)
            {
                var payload = parts[1];
                // Base64 padding düzeltmesi
                payload = payload.Replace('-', '+').Replace('_', '/');
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                var jsonBytes = Convert.FromBase64String(payload);
                var json = System.Text.Encoding.UTF8.GetString(jsonBytes);
                var doc = System.Text.Json.JsonDocument.Parse(json);

                var role   = doc.RootElement.TryGetProperty("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out var r) ? r.GetString() ?? "" : "";
                var userId = doc.RootElement.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out var u) ? u.GetString() ?? "" : "";

                var roleCookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddMinutes(120), Secure = false };
                Response.Cookies.Append("UserRole", role, roleCookieOptions);
                Response.Cookies.Append("UserId", userId, roleCookieOptions);
            }

            return RedirectToAction("Index", "Ticket");
        }

        ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
        return View(model);
    }
    
    [HttpGet]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("UserRole");
        Response.Cookies.Delete("UserId");
        return RedirectToAction("Login");
    }
}
