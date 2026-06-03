namespace HitTrackerAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MuscleGroup { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int RequiredRestDays { get; set; }
        public string? Equipment { get; set; }
        public bool IsCustom { get; set; } = false;
        public int? UserId { get; set; }
        public string? Level { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}