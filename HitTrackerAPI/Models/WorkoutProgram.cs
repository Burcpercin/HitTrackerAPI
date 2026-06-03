namespace HitTrackerAPI.Models
{
    public class WorkoutProgram
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Goal { get; set; } = string.Empty;
        public int DaysPerWeek { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
        public ICollection<ProgramExercise> Exercises { get; set; } = new List<ProgramExercise>();
    }
}