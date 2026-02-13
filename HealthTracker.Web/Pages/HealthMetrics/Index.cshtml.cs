//using HealthTracker.Web.Models.DTO;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace HealthTracker.Web.Pages.HealthMetrics
//{
//    public class IndexModel : PageModel
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
//        public IndexModel(IHttpClientFactory httpClientFactory)
//        {
//            _httpClientFactory = httpClientFactory;
//        }

//        public List<HealthMetricDto> Metrics { get; set; } = new();

//        public async Task <IActionResult> OnGetAsync()
//        {
//            var token = HttpContext.Request.Cookies["auth_token"];
//            if (string.IsNullOrEmpty(token))
//            {
//                return RedirectToPage("/Auth/Login");
//            }

//            var client
//        }
//    }
//}
