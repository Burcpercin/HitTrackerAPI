namespace HitTrackerAPI.Services
{
    public class CalorieService : ICalorieService
    {
        // Aktivite çarpanları
        private static readonly Dictionary<string, float> ActivityMultipliers = new()
        {
            { "sedentary",   1.20f },
            { "light",       1.375f },
            { "moderate",    1.55f },
            { "active",      1.725f },
            { "very_active", 1.90f }
        };

        // Hedef kalori ayarlamaları
        private static readonly Dictionary<string, int> GoalAdjustments = new()
        {
            { "muscle_gain", +400 },
            { "fat_loss",    -500 },
            { "strength",       0 },
            { "endurance",      0 }
        };

        public CalorieReport Calculate(float weightKg, float heightCm,
            int age, string gender, string activityLevel, string goal)
        {
            // Mifflin-St Jeor formülü
            var bmr = (int)((10 * weightKg) + (6.25f * heightCm) - (5 * age)
                + (gender == "male" ? 5 : -161));

            // TDEE
            var multiplier = ActivityMultipliers.GetValueOrDefault(activityLevel, 1.55f);
            var tdee = (int)(bmr * multiplier);

            // Hedef kalori
            var adjustment = GoalAdjustments.GetValueOrDefault(goal, 0);
            var target = tdee + adjustment;

            // Makrolar
            var macros = CalculateMacros(target, weightKg, goal);

            // Özet mesaj
            var summary = BuildSummary(target, goal);

            return new CalorieReport(bmr, tdee, target, macros, goal, summary);
        }

        private static MacroReport CalculateMacros(
            int targetCalories, float weightKg, string goal)
        {
            // Protein: vücut ağırlığına göre
            // Muscle gain: 2.2g/kg, diğer: 1.8g/kg
            var proteinMultiplier = goal == "muscle_gain" ? 2.2f : 1.8f;
            var proteinG = (int)(weightKg * proteinMultiplier);
            var proteinCals = proteinG * 4;

            // Yağ: toplam kalorinin %25'i
            var fatCals = (int)(targetCalories * 0.25f);
            var fatG = fatCals / 9;

            // Karbonhidrat: kalan kalori
            var carbCals = targetCalories - proteinCals - fatCals;
            var carbsG = carbCals / 4;

            return new MacroReport(proteinG, carbsG < 0 ? 0 : carbsG, fatG);
        }

        private static string BuildSummary(int targetCalories, string goal)
        {
            return goal switch
            {
                "muscle_gain" => $"Eat {targetCalories} kcal/day. Caloric surplus for muscle growth.",
                "fat_loss"    => $"Eat {targetCalories} kcal/day. Caloric deficit for fat loss.",
                "strength"    => $"Eat {targetCalories} kcal/day. Maintenance — pure strength focus.",
                "endurance"   => $"Eat {targetCalories} kcal/day. Maintenance — endurance focus.",
                _             => $"Eat {targetCalories} kcal/day."
            };
        }
    }
}