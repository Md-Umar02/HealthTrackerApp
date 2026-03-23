using HealthTracker.Web.Models.Common;
using HealthTracker.Web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HealthTracker.Web.Pages.HealthMetrics;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public UpdateHealthMetricDto Metric { get; set; }

    public List<HealthMetricDto> MetricTypes { get; set; } = new();

    public async Task OnGetAsync(int id)
    {
        var token = HttpContext.Request.Cookies["auth_token"];

        var client = _httpClientFactory.CreateClient("HealthTrackerApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"api/HealthMetric/{id}");

        var json = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<UpdateHealthMetricDto>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Metric = apiResponse.Result;

        await LoadMetricTypes(client);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var token = HttpContext.Request.Cookies["auth_token"];

        var client = _httpClientFactory.CreateClient("HealthTrackerApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var content = new StringContent(
            JsonSerializer.Serialize(Metric),
            Encoding.UTF8,
            "application/json");

        await client.PutAsync($"api/HealthMetric/Update/{Metric.Id}", content);

        TempData["SuccessMessage"] = "Metric updated successfully!";

        return RedirectToPage("/HealthMetrics/Index");
    }

    private async Task LoadMetricTypes(HttpClient client)
    {
        var response = await client.GetAsync("api/MetricType");

        var json = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<HealthMetricDto>>>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        MetricTypes = apiResponse.Result;
    }
}