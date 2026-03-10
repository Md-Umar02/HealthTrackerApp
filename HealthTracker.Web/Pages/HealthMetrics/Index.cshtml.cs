using HealthTracker.Web.Models.Common;
using HealthTracker.Web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HealthTracker.Web.Pages.HealthMetrics
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<HealthMetricDto> Metrics { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Request.Cookies["auth_token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _httpClientFactory.CreateClient("HealthTrackerApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/healthmetric/me");

            if (!response.IsSuccessStatusCode)
            {
                // Handle error (e.g., log it, show an error message, etc.)
                return RedirectToPage("/Auth/Login");
            }

            var json = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<HealthMetricDto>>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Metrics = apiResponse.Result ?? new List<HealthMetricDto>();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = HttpContext.Request.Cookies["auth_token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }
            var client = _httpClientFactory.CreateClient("HealthTrackerApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"/api/healthmetric/Delete/{id}");
            if (response.IsSuccessStatusCode)
            {
                // Handle error (e.g., log it, show an error message, etc.)
                TempData["SuccessMessage"] = "Deleted Successfully";
            }

            else
            {
                TempData["ErrorMessage"] = "Failed to delete metric";
            }

            return RedirectToPage();
        }
    }
}
