namespace HitTrackerAPI.Services
{
    public interface ICalorieService
    {
        CalorieReport Calculate(float weightKg, float heightCm,
            int age, string gender, string activityLevel, string goal);
    }

    public record MacroReport(int ProteinG, int CarbsG, int FatG);

    public record CalorieReport(
        int Bmr,
        int Tdee,
        int TargetCalories,
        MacroReport Macros,
        string Goal,
        string Summary
    );
}