using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Telecom.UI.Models;

namespace Telecom.UI.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        
        // Base address will be configured in Program.cs
    }

    private void AddAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<string?> LoginAsync(LoginViewModel model)
    {
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/auth/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(result);
            return jsonDoc.RootElement.GetProperty("token").GetString();
        }
        return null;
    }

    public async Task<List<TicketViewModel>> GetTicketsAsync()
    {
        AddAuthorizationHeader();
        var response = await _httpClient.GetAsync("api/tickets");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<TicketViewModel>>(content, options) ?? new List<TicketViewModel>();
        }
        return new List<TicketViewModel>();
    }

    public async Task<bool> CreateTicketAsync(CreateTicketViewModel model)
    {
        AddAuthorizationHeader();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/tickets", content);
        return response.IsSuccessStatusCode;
    }
}
