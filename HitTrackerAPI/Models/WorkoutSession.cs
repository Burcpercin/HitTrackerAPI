namespace HitTrackerAPI.Models
{
    public class WorkoutSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime SessionDate { get; set; }
        public string? Notes { get; set; }
        public int? DurationMinutes { get; set; }
        public int? ProgramId { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public WorkoutProgram? Program { get; set; }
        public ICollection<SessionExercise> Exercises { get; set; } = new List<SessionExercise>();
    }
}