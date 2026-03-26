using System.Net.Http.Headers;
using System.Text.Json;
using HealthTracker.Web.Models.Common;
using HealthTracker.Web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

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

        public string AIResponse { get; set; }

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
            try
            {
                var response = await client.GetAsync("api/User/me");

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Auth/Login");
                }

                // 5️⃣ Read response
                var json = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserProfileDto>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse != null)
                {
                    User = apiResponse.Result;
                }
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostGetInsightsAsync()
        {
            // 1️⃣ Read JWT from cookie
            var token = HttpContext.Request.Cookies["auth_token"];
            if (string.IsNullOrEmpty(token))
            {
                return new JsonResult(new { error = "Not authenticated", redirect = "/Auth/Login" });
            }

            // 2️⃣ Create API client
            var client = _httpClientFactory.CreateClient("HealthTrackerApi");

            // 3️⃣ Attach JWT
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync("api/AI/Insights");

                if (!response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { error = "Failed to get insights." });
                }

                var result = await response.Content.ReadAsStringAsync();

                // Parse and extract the actual text content
                string extractedText = ExtractInsightText(result);

                // Format the text for HTML display
                string formattedText = FormatInsightText(extractedText);

                return new JsonResult(new { insights = formattedText });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "An error occurred while fetching insights: " + ex.Message });
            }
        }

        private string ExtractInsightText(string jsonResponse)
        {
            try
            {
                // First, try to parse as JSON
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                // Check if the root is a string (direct text response)
                if (root.ValueKind == JsonValueKind.String)
                {
                    string stringValue = root.GetString();
                    // Try to parse if it's a nested JSON string
                    if (!string.IsNullOrEmpty(stringValue) && (stringValue.Trim().StartsWith("{") || stringValue.Trim().StartsWith("[")))
                    {
                        try
                        {
                            using JsonDocument innerDoc = JsonDocument.Parse(stringValue);
                            return ExtractTextFromAnyStructure(innerDoc.RootElement);
                        }
                        catch
                        {
                            return stringValue;
                        }
                    }
                    return stringValue ?? "No insights available.";
                }

                // Try to find text in the JSON structure
                return ExtractTextFromAnyStructure(root);
            }
            catch (JsonException)
            {
                // If it's not valid JSON, return as is (might be plain text)
                return jsonResponse;
            }
            catch (Exception ex)
            {
                return "Error parsing insights: " + ex.Message;
            }
        }

        private string ExtractTextFromAnyStructure(JsonElement element)
        {
            // Try to get the text from various possible paths

            // Path 1: candidates[0].content.parts[0].text (Gemini format)
            if (element.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                if (firstCandidate.TryGetProperty("content", out var content))
                {
                    if (content.TryGetProperty("parts", out var parts) && parts.GetArrayLength() > 0)
                    {
                        var firstPart = parts[0];
                        if (firstPart.TryGetProperty("text", out var text))
                        {
                            return text.GetString() ?? "";
                        }
                    }
                }
            }

            // Path 2: Direct text property
            if (element.TryGetProperty("text", out var directText))
            {
                return directText.GetString() ?? "";
            }

            // Path 3: insights property
            if (element.TryGetProperty("insights", out var insights))
            {
                if (insights.ValueKind == JsonValueKind.String)
                {
                    return insights.GetString() ?? "";
                }
                return insights.ToString();
            }

            // Path 4: content property
            if (element.TryGetProperty("content", out var contentProp))
            {
                if (contentProp.ValueKind == JsonValueKind.String)
                {
                    return contentProp.GetString() ?? "";
                }
                return contentProp.ToString();
            }

            // Path 5: response property
            if (element.TryGetProperty("response", out var responseProp))
            {
                if (responseProp.ValueKind == JsonValueKind.String)
                {
                    return responseProp.GetString() ?? "";
                }
                return responseProp.ToString();
            }

            // Path 6: message property
            if (element.TryGetProperty("message", out var message))
            {
                if (message.ValueKind == JsonValueKind.String)
                {
                    return message.GetString() ?? "";
                }
                return message.ToString();
            }

            // Path 7: If it's a string value
            if (element.ValueKind == JsonValueKind.String)
            {
                return element.GetString() ?? "";
            }

            // Path 8: If it's an object with a single string property
            if (element.ValueKind == JsonValueKind.Object)
            {
                var properties = element.EnumerateObject().ToList();
                if (properties.Count == 1 && properties[0].Value.ValueKind == JsonValueKind.String)
                {
                    return properties[0].Value.GetString() ?? "";
                }
            }

            // Last resort: return the raw JSON as string
            return element.ToString();
        }

        private string FormatInsightText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "<p>No insights available at this time.</p>";
            }

            // First, unescape any JSON escape sequences
            text = System.Text.RegularExpressions.Regex.Unescape(text);

            // Remove any remaining JSON artifacts
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\\u0022", "\"");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\\n", "\n");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\\r", "");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\\t", "    ");

            // Convert markdown-style formatting to HTML

            // Headers (### Header)
            text = System.Text.RegularExpressions.Regex.Replace(text, @"###\s+(.+?)(?:\n|$)", "<h3>$1</h3>\n", System.Text.RegularExpressions.RegexOptions.Multiline);
            text = System.Text.RegularExpressions.Regex.Replace(text, @"##\s+(.+?)(?:\n|$)", "<h2>$1</h2>\n", System.Text.RegularExpressions.RegexOptions.Multiline);
            text = System.Text.RegularExpressions.Regex.Replace(text, @"#\s+(.+?)(?:\n|$)", "<h1>$1</h1>\n", System.Text.RegularExpressions.RegexOptions.Multiline);

            // Bold text
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\*\*(.+?)\*\*", "<strong>$1</strong>");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"__(.+?)__", "<strong>$1</strong>");

            // Italic text
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\*(.+?)\*", "<em>$1</em>");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"_(.+?)_", "<em>$1</em>");

            // Bullet points - convert * or - to list items
            var lines = text.Split('\n');
            var inList = false;
            var resultLines = new List<string>();

            foreach (var line in lines)
            {
                // Check for bullet points
                var bulletMatch = System.Text.RegularExpressions.Regex.Match(line, @"^\s*[\*\-]\s+(.+)$");
                if (bulletMatch.Success)
                {
                    if (!inList)
                    {
                        resultLines.Add("<ul>");
                        inList = true;
                    }
                    resultLines.Add($"<li>{bulletMatch.Groups[1].Value}</li>");
                }
                else
                {
                    if (inList)
                    {
                        resultLines.Add("</ul>");
                        inList = false;
                    }
                    resultLines.Add(line);
                }
            }

            if (inList)
            {
                resultLines.Add("</ul>");
            }

            text = string.Join("\n", resultLines);

            // Numbered lists - Fixed regex pattern
            var numberedLines = text.Split('\n');
            var inNumberedList = false;
            var numberedResultLines = new List<string>();

            foreach (var line in numberedLines)
            {
                // Match numbered list items like "1. item" or "2. item"
                var numberedMatch = System.Text.RegularExpressions.Regex.Match(line, @"^\s*(\d+)\.\s+(.+)$");
                if (numberedMatch.Success)
                {
                    if (!inNumberedList)
                    {
                        numberedResultLines.Add("<ol>");
                        inNumberedList = true;
                    }
                    numberedResultLines.Add($"<li>{numberedMatch.Groups[2].Value}</li>");
                }
                else
                {
                    if (inNumberedList)
                    {
                        numberedResultLines.Add("</ol>");
                        inNumberedList = false;
                    }
                    numberedResultLines.Add(line);
                }
            }

            if (inNumberedList)
            {
                numberedResultLines.Add("</ol>");
            }

            text = string.Join("\n", numberedResultLines);

            // Horizontal rules
            text = System.Text.RegularExpressions.Regex.Replace(text, @"^---$", "<hr />", System.Text.RegularExpressions.RegexOptions.Multiline);
            text = System.Text.RegularExpressions.Regex.Replace(text, @"^___$", "<hr />", System.Text.RegularExpressions.RegexOptions.Multiline);

            // Convert double newlines to paragraphs
            var paragraphs = text.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var formattedParagraphs = new List<string>();

            foreach (var para in paragraphs)
            {
                var trimmed = para.Trim();
                if (!string.IsNullOrEmpty(trimmed) &&
                    !trimmed.StartsWith("<") &&
                    !trimmed.StartsWith("</") &&
                    !trimmed.StartsWith("<ul") &&
                    !trimmed.StartsWith("</ul") &&
                    !trimmed.StartsWith("<ol") &&
                    !trimmed.StartsWith("</ol") &&
                    !trimmed.StartsWith("<li") &&
                    !trimmed.StartsWith("</li") &&
                    !trimmed.StartsWith("<h") &&
                    !trimmed.StartsWith("</h") &&
                    !trimmed.StartsWith("<hr"))
                {
                    formattedParagraphs.Add($"<p>{trimmed}</p>");
                }
                else
                {
                    formattedParagraphs.Add(trimmed);
                }
            }

            text = string.Join("\n", formattedParagraphs);

            // Replace remaining newlines with <br/> (but not inside HTML tags)
            var linesForBreaks = text.Split('\n');
            var breakResult = new List<string>();

            foreach (var line in linesForBreaks)
            {
                if (!line.Trim().StartsWith("<") || line.Trim().StartsWith("</"))
                {
                    breakResult.Add(line);
                }
                else
                {
                    breakResult.Add(line);
                }
            }

            text = string.Join("<br/>", breakResult);

            // Clean up any double <br/> tags
            text = System.Text.RegularExpressions.Regex.Replace(text, @"(<br/>\s*){2,}", "<br/><br/>");

            return text;
        }
    }
}