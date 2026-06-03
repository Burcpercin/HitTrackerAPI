using HitTrackerAPI.Models;

namespace HitTrackerAPI.Services
{
    public interface ISessionService
    {
        Task<IEnumerable<WorkoutSession>> GetAllAsync(int userId);
        Task<WorkoutSession?> GetByIdAsync(int id);
        Task<WorkoutSession> CreateAsync(int userId, DateTime sessionDate, int? programId);
        Task DeleteAsync(int id, int userId);
        Task<SessionExercise> LogExerciseAsync(int sessionId, int exerciseId,
            float weightKg, int reps, bool reachedFailure);
    }
}