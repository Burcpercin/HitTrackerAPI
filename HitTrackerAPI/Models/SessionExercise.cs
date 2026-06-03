using System.Text.Json.Serialization;

namespace HitTrackerAPI.Models
{
    public class SessionExercise
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int ExerciseId { get; set; }
        public float WeightKg { get; set; }
        public int Reps { get; set; }
        public bool ReachedFailure { get; set; } = false;

        // Navigation
        [JsonIgnore]
        public WorkoutSession Session { get; set; } = null!;
        
        public Exercise Exercise { get; set; } = null!;
    }
}