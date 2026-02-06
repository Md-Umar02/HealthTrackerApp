using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace HealthTracker.Web.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        public void OnGet()
        {
        }
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public int Age { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("HealthTrackerApi");

            var request = new
            {
                name = Name,
                email = Email,
                password = Password,
                age = Age
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("api/Auth/register", content);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Registration failed";
                return Page();
            }

            // On success → redirect to Login
            return RedirectToPage("/Auth/Login");
        }
    }
}

