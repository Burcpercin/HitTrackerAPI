using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public interface IProgramRepository
    {
        Task<IEnumerable<WorkoutProgram>> GetAllAsync(int userId);
        Task<WorkoutProgram?> GetByIdAsync(int id);
        Task<WorkoutProgram> CreateAsync(WorkoutProgram program);
        Task<WorkoutProgram> UpdateAsync(WorkoutProgram program);
        Task DeleteAsync(int id);
        Task SetActiveAsync(int programId, int userId);
        Task<ProgramExercise> AddExerciseAsync(ProgramExercise programExercise);
        Task RemoveExerciseAsync(int entryId);
    }
}