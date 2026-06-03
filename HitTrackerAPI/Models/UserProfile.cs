namespace HitTrackerAPI.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime BirthDate { get; set; }
        public float WeightKg { get; set; }
        public float HeightCm { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = string.Empty;

        // Navigation
        public User User { get; set; } = null!;
    }
}