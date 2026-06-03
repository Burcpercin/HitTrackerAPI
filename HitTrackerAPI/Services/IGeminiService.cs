namespace HitTrackerAPI.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateWorkoutSuggestionAsync(
            string goal, string experienceLevel,
            string equipment, int daysPerWeek,
            string? injuries = null);
    }
}