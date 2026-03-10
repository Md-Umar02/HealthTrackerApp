using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HealthTracker.Web.Pages.HealthMetrics
{
    public class CreateModel : PageModel
    {
        public void OnGet()
        {
        }
        private readonly IHttpClientFactory _httpClientFactory;
        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public int MetricTypeId { get; set; }
        [BindProperty]
        public string Value { get; set; }
        public async Task<IActionResult> OnPostAsync() 
        {
            var token = HttpContext.Request.Cookies["auth_token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            var client = _httpClientFactory.CreateClient("HealthTrackerApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                metricTypeId = MetricTypeId,
                value = Value
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("api/HealthMetric", content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to add metric");
                return Page();
            }

            TempData["SuccessMessage"] = "Saved Successfully";
            return RedirectToPage("/HealthMetrics/Index");
        }
    }
}
