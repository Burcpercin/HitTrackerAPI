using System.Text.Json.Serialization;

namespace HitTrackerAPI.Models
{
    public class ProgramExercise
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int ExerciseId { get; set; }
        public int DayOfWeek { get; set; }
        public int Sets { get; set; }
        public int TargetReps { get; set; }
        public string? Notes { get; set; }

        [JsonIgnore]
        public WorkoutProgram? Program { get; set; }
        
        public Exercise? Exercise { get; set; }  // Exercise bilgisini frontend görsün
    }
}