namespace HitTrackerAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public UserProfile? Profile { get; set; }
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
        public ICollection<WorkoutProgram> Programs { get; set; } = new List<WorkoutProgram>();
        public ICollection<WorkoutSession> Sessions { get; set; } = new List<WorkoutSession>();
    }
}