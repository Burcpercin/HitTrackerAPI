using System.ComponentModel.DataAnnotations;

namespace HitTrackerAPI.DTOs
{
    public class ProfileRequest
    {
        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; set; }

        [Range(30, 300, ErrorMessage = "Weight must be between 30 and 300 kg")]
        public float WeightKg { get; set; }

        [Range(100, 250, ErrorMessage = "Height must be between 100 and 250 cm")]
        public float HeightCm { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(male|female)$", ErrorMessage = "Gender must be male or female")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Activity level is required")]
        [RegularExpression("^(sedentary|light|moderate|active|very_active)$",
            ErrorMessage = "Invalid activity level")]
        public string ActivityLevel { get; set; } = string.Empty;
    }
}