using System.Text;
using System.Text.Json;

namespace HitTrackerAPI.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GeminiService(HttpClient http, IConfiguration config)
        {
            _http   = http;
            _apiKey = config["Gemini:ApiKey"]!;
        }

        public async Task<string> GenerateWorkoutSuggestionAsync(
            string goal, string experienceLevel,
            string equipment, int daysPerWeek,
            string? injuries = null)
        {
            var prompt = $@"
You are a fitness coach specializing in Mike Mentzer's High Intensity Training.
Create a COMPLETE workout program for this person.

PROFILE:
- Goal: {goal}
- Experience: {experienceLevel}
- Equipment: {equipment}
- Training days per week: {daysPerWeek}
{(injuries != null ? $"- Injuries: {injuries}" : "")}

RULES:
- HIT style: 1-2 sets per exercise, train to failure
- Max 4-5 exercises per day
- Include rest days between muscle groups

OUTPUT FORMAT (be concise, follow exactly):

PROGRAM: [name]

[DAY NAME] — [Muscle Groups]
- Exercise Name — X sets × X reps

REST DAYS: [list rest days]

KEY PRINCIPLE: [one sentence from Mentzer philosophy]

Nothing else. No intro, no explanations.
";

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

            var json    = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url     = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var response = await _http.PostAsync(url, content);
            var result   = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Gemini response: " + result);

            using var doc  = JsonDocument.Parse(result);
            var suggestion = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return suggestion ?? "Could not generate suggestion";
        }
    }
}