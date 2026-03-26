using HealthTracker.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : Controller
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet("Insights")]
        public async Task<IActionResult> GetInsights()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _aiService.GetHealthInsightsAsync(userId);

            return Ok(result);
        }
    }
}
