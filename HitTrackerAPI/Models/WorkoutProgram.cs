using System.Text.Json.Serialization;

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
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        public List<ProgramExercise> Exercises { get; set; } = new();
    }
}