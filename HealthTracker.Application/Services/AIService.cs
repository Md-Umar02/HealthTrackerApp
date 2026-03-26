using HealthTracker.Application.Services.Interface;
using HealthTracker.Domain.Contracts;
using HealthTracker.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Application.Services
{
    public class AIService : IAIService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHealthMetricRepository _healthMetricRepository;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public AIService(IUserRepository userRepository, IHealthMetricRepository healthMetricRepository, IConfiguration config, HttpClient httpClient)
        {
            _userRepository = userRepository;
            _healthMetricRepository = healthMetricRepository;
            _config = config;
            _httpClient = httpClient;
        }
        public async Task<string> GetHealthInsightsAsync(string identityUserId)
        {
            var user = await _userRepository.GetByIdentityIdAsync(identityUserId);
            var metrics = await _healthMetricRepository.GetByUserIdAsync(user.Id);

            var prompt = BuildPrompt(metrics);

            var apiKey = _config["Gemini:ApiKey"];

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"AI API request failed: {response.StatusCode}, {errorContent}");
            }

            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        private string BuildPrompt(IEnumerable<HealthMetric> metrics)
        {
            var text = "Analyze the following health data:\n";

            foreach (var metric in metrics)
            {
                text += $"- {metric.MetricType.Name}: {metric.Value} (recorded at {metric.RecordedAt})\n";
            }

            text += "Provide insights and recommendations based on this data.";

            return text;
        }
    }
}
