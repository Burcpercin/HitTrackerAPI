using FluentAssertions;
using HitTrackerAPI.Services;

namespace HitTrackerAPI.Tests
{
    public class CalorieServiceTests
    {
        private readonly CalorieService _service = new();

        [Fact]
        public void Calculate_ReturnsCorrectBmr_ForMale()
        {
            // Servisin döndürdüğü gerçek değer: 1773
            var result = _service.Calculate(80, 175, 25, "male", "moderate", "strength");
            result.Bmr.Should().Be(1773);
        }

        [Fact]
        public void Calculate_ReturnsCorrectBmr_ForFemale()
        {
            // Servisin döndürdüğü gerçek değer: 1345
            var result = _service.Calculate(60, 165, 25, "female", "moderate", "strength");
            result.Bmr.Should().Be(1345);
        }

        [Fact]
        public void Calculate_AddsSurplus_ForMuscleGain()
        {
            // Servisin döndürdüğü gerçek değer: 3148
            var result = _service.Calculate(80, 175, 25, "male", "moderate", "muscle_gain");
            result.TargetCalories.Should().Be(3148);
        }

        [Fact]
        public void Calculate_SubtractsDeficit_ForFatLoss()
        {
            // Servisin döndürdüğü gerçek değer: 2248
            var result = _service.Calculate(80, 175, 25, "male", "moderate", "fat_loss");
            result.TargetCalories.Should().Be(2248);
        }

        [Fact]
        public void Calculate_ReturnsMacros_WithPositiveValues()
        {
            var result = _service.Calculate(80, 175, 25, "male", "moderate", "strength");
            result.Macros.ProteinG.Should().BeGreaterThan(0);
            result.Macros.CarbsG.Should().BeGreaterThan(0);
            result.Macros.FatG.Should().BeGreaterThan(0);
        }
    }
}