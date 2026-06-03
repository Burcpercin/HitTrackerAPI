using System.ComponentModel.DataAnnotations;

namespace HitTrackerAPI.DTOs
{
    public class CreateExerciseRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        [MaxLength(100, ErrorMessage = "Name max 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Muscle group is required")]
        public string MuscleGroup { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description max 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Rest days is required")]
        [Range(3, 14, ErrorMessage = "Rest days must be between 3 and 14 (Mentzer principle)")]
        public int RequiredRestDays { get; set; }

        public string? Equipment { get; set; }
    }

    public class UpdateExerciseRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Muscle group is required")]
        public string MuscleGroup { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(3, 14, ErrorMessage = "Rest days must be between 3 and 14")]
        public int RequiredRestDays { get; set; }
    }
}