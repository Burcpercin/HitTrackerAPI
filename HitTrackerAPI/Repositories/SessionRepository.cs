using Microsoft.EntityFrameworkCore;
using HitTrackerAPI.Data;
using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly AppDbContext _db;

        public SessionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<WorkoutSession>> GetAllAsync(int userId)
        {
            return await _db.WorkoutSessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Program)
                .Include(s => s.Exercises)
                    .ThenInclude(se => se.Exercise)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        public async Task<WorkoutSession?> GetByIdAsync(int id)
        {
            return await _db.WorkoutSessions
                .Include(s => s.Program)
                .Include(s => s.Exercises)
                    .ThenInclude(se => se.Exercise)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<WorkoutSession> CreateAsync(WorkoutSession session)
        {
            _db.WorkoutSessions.Add(session);
            await _db.SaveChangesAsync();
            return session;
        }

        public async Task DeleteAsync(int id)
        {
            var session = await _db.WorkoutSessions.FindAsync(id);
            if (session != null)
            {
                _db.WorkoutSessions.Remove(session);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<SessionExercise> AddExerciseAsync(SessionExercise se)
        {
            _db.SessionExercises.Add(se);
            await _db.SaveChangesAsync();
            return se;
        }
    }
}