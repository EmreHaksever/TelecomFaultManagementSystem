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

    private async Task SetAuthorizationHeaderAsync()
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
        await SetAuthorizationHeaderAsync();
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
        await SetAuthorizationHeaderAsync();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/tickets", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<TechnicianViewModel>> GetTechniciansAsync()
    {
        await SetAuthorizationHeaderAsync();
        var response = await _httpClient.GetAsync("api/auth/technicians");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<TechnicianViewModel>>(content, options) ?? new List<TechnicianViewModel>();
        }
        return new List<TechnicianViewModel>();
    }

    public async Task<bool> AssignTechnicianAsync(Guid ticketId, Guid technicianId)
    {
        await SetAuthorizationHeaderAsync();
        var model = new { TicketId = ticketId, TechnicianId = technicianId };
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var content = new StringContent(JsonSerializer.Serialize(model, options), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("api/tickets/assign", content);
        return response.IsSuccessStatusCode;
    }
    public async Task<bool> ResolveTicketAsync(Guid ticketId, string resolutionDetail)
    {
        await SetAuthorizationHeaderAsync();
        var model = new { TicketId = ticketId, Status = 3, ResolutionDetail = resolutionDetail }; // 3 = Resolved
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var content = new StringContent(JsonSerializer.Serialize(model, options), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PutAsync("api/tickets/status", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<TicketViewModel?> GetTicketByIdAsync(Guid id)
    {
        await SetAuthorizationHeaderAsync();
        var response = await _httpClient.GetAsync($"api/tickets/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<TicketViewModel>(content, options);
        }
        return null;
    }
}
