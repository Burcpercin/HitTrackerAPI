using HitTrackerAPI.Models;
using HitTrackerAPI.Repositories;

namespace HitTrackerAPI.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _repo;

        public SessionService(ISessionRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<WorkoutSession>> GetAllAsync(int userId)
            => await _repo.GetAllAsync(userId);

        public async Task<WorkoutSession?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task<WorkoutSession> CreateAsync(int userId,
            DateTime sessionDate, int? programId)
        {
            var session = new WorkoutSession
            {
                UserId      = userId,
                SessionDate = sessionDate,
                ProgramId   = programId
            };

            return await _repo.CreateAsync(session);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var session = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Session not found");

            if (session.UserId != userId)
                throw new Exception("Not authorized");

            await _repo.DeleteAsync(id);
        }

        public async Task<SessionExercise> LogExerciseAsync(int sessionId,
            int exerciseId, float weightKg, int reps, bool reachedFailure)
        {
            if (weightKg <= 0) throw new Exception("Weight must be greater than 0");
            if (reps <= 0)     throw new Exception("Reps must be greater than 0");

            var se = new SessionExercise
            {
                SessionId      = sessionId,
                ExerciseId     = exerciseId,
                WeightKg       = weightKg,
                Reps           = reps,
                ReachedFailure = reachedFailure
            };

            return await _repo.AddExerciseAsync(se);
        }
    }
}