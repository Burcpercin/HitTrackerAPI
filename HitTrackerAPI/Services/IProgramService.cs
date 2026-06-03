using HitTrackerAPI.Models;

namespace HitTrackerAPI.Services
{
    public interface IProgramService
    {
        Task<IEnumerable<WorkoutProgram>> GetAllAsync(int userId);
        Task<WorkoutProgram?> GetByIdAsync(int id);
        Task<WorkoutProgram> CreateAsync(int userId, string name,
            string goal, int daysPerWeek);
        Task DeleteAsync(int id, int userId);
        Task SetActiveAsync(int id, int userId);
        Task<ProgramExercise> AddExerciseAsync(int programId, int exerciseId,
            int dayOfWeek, int sets, int targetReps);
        Task RemoveExerciseAsync(int programId, int entryId);
    }
}