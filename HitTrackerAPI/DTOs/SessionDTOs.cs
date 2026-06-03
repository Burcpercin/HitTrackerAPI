using System.ComponentModel.DataAnnotations;

namespace HitTrackerAPI.DTOs
{
    public class CreateSessionRequest
    {
        [Required(ErrorMessage = "Session date is required")]
        public DateTime SessionDate { get; set; }

        public int? ProgramId { get; set; }
    }

    public class LogExerciseRequest
    {
        [Required(ErrorMessage = "Exercise ID is required")]
        public int ExerciseId { get; set; }

        [Range(0.5, 1000, ErrorMessage = "Weight must be between 0.5 and 1000 kg")]
        public float WeightKg { get; set; }

        [Range(1, 1000, ErrorMessage = "Reps must be between 1 and 1000")]
        public int Reps { get; set; }

        public bool ReachedFailure { get; set; }
    }
}