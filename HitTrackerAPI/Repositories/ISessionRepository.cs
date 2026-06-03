using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public interface ISessionRepository
    {
        Task<IEnumerable<WorkoutSession>> GetAllAsync(int userId);
        Task<WorkoutSession?> GetByIdAsync(int id);
        Task<WorkoutSession> CreateAsync(WorkoutSession session);
        Task DeleteAsync(int id);
        Task<SessionExercise> AddExerciseAsync(SessionExercise sessionExercise);
    }
}