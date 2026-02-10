using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HealthTracker.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("HealthTrackerApi");

            // Match the property names your API expects (usually camelCase)
            var loginRequest = new
            {
                email = Email,
                password = Password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await client.PostAsync("api/Auth/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Invalid login attempt.";
                    return Page();
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize into our Wrapper Model
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper>(responseContent, options);

                // Dig into the 'Result' object to find the token
                if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Result != null)
                {
                    // Since Result is an 'object' or 'JsonElement', we convert it to our LoginData class
                    var resultJson = apiResponse.Result.ToString();
                    var loginData = JsonSerializer.Deserialize<LoginData>(resultJson, options);

                    if (!string.IsNullOrEmpty(loginData?.Token))
                    {
                        // Save the JWT to a secure HttpOnly cookie
                        HttpContext.Response.Cookies.Append(
                            "auth_token",
                            loginData.Token,
                            new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true, // Ensure your app is running on HTTPS
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTimeOffset.UtcNow.AddHours(1)
                            });

                        return RedirectToPage("/Profile/Index");
                    }
                }

                ErrorMessage = apiResponse?.DisplayMessage ?? "Login failed: No token received.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while connecting to the server.";
                return Page();
            }
        }

        // --- Helper Models to match your Clean Architecture API Structure ---

        private class ApiResponseWrapper
        {
            public bool IsSuccess { get; set; }
            public object Result { get; set; } // This holds the User/Token object
            public string DisplayMessage { get; set; }
            public int StatusCode { get; set; }
        }

        private class LoginData
        {
            public string Token { get; set; }
            public string Email { get; set; }
        }
    }
}