using System.ComponentModel.DataAnnotations;

namespace HitTrackerAPI.DTOs
{
    public class CreateProgramRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Program name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Name max 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Goal is required")]
        public string Goal { get; set; } = string.Empty;

        [Range(1, 7, ErrorMessage = "Days per week must be between 1 and 7")]
        public int DaysPerWeek { get; set; }
    }

    public class AddExerciseRequest
    {
        [Required(ErrorMessage = "Exercise ID is required")]
        public int ExerciseId { get; set; }

        [Range(1, 7, ErrorMessage = "Day of week must be between 1 and 7")]
        public int DayOfWeek { get; set; }

        [Range(1, 10, ErrorMessage = "Sets must be between 1 and 10")]
        public int Sets { get; set; }

        [Range(1, 100, ErrorMessage = "Target reps must be between 1 and 100")]
        public int TargetReps { get; set; }
    }
}