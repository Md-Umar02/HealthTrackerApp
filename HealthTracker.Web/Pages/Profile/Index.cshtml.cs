using System.Net.Http.Headers;
using System.Text.Json;
using HealthTracker.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthTracker.Web.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public UserProfileDto User { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // 1️⃣ Read JWT from cookie
            var token = HttpContext.Request.Cookies["auth_token"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Auth/Login");
            }

            // 2️⃣ Create API client
            var client = _httpClientFactory.CreateClient("HealthTrackerApi");

            // 3️⃣ Attach JWT
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // 4️⃣ Call API
            var response = await client.GetAsync("api/User/me");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Auth/Login");
            }

            // 5️⃣ Read response
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserProfileDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            User = apiResponse.Result;

            return Page();
        }
        public class ApiResponse<T>
        {
            public T Result { get; set; }
        }
    }
}