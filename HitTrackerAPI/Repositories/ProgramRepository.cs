using Microsoft.EntityFrameworkCore;
using HitTrackerAPI.Data;
using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public class ProgramRepository : IProgramRepository
    {
        private readonly AppDbContext _db;

        public ProgramRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<WorkoutProgram>> GetAllAsync(int userId)
        {
            return await _db.WorkoutPrograms
                .Where(p => p.UserId == userId)
                .Include(p => p.Exercises)
                    .ThenInclude(pe => pe.Exercise)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<WorkoutProgram?> GetByIdAsync(int id)
        {
            return await _db.WorkoutPrograms
                .Include(p => p.Exercises)
                    .ThenInclude(pe => pe.Exercise)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<WorkoutProgram> CreateAsync(WorkoutProgram program)
        {
            _db.WorkoutPrograms.Add(program);
            await _db.SaveChangesAsync();
            return program;
        }

        public async Task<WorkoutProgram> UpdateAsync(WorkoutProgram program)
        {
            _db.WorkoutPrograms.Update(program);
            await _db.SaveChangesAsync();
            return program;
        }

        public async Task DeleteAsync(int id)
        {
            var program = await _db.WorkoutPrograms.FindAsync(id);
            if (program != null)
            {
                _db.WorkoutPrograms.Remove(program);
                await _db.SaveChangesAsync();
            }
        }

        // Transaction ile — önce hepsini pasifleştir, sonra seçileni aktifleştir
        public async Task SetActiveAsync(int programId, int userId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                await _db.WorkoutPrograms
                    .Where(p => p.UserId == userId)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsActive, false));

                await _db.WorkoutPrograms
                    .Where(p => p.Id == programId)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsActive, true));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ProgramExercise> AddExerciseAsync(ProgramExercise pe)
        {
            _db.ProgramExercises.Add(pe);
            await _db.SaveChangesAsync();
            return pe;
        }

        public async Task RemoveExerciseAsync(int entryId)
        {
            var entry = await _db.ProgramExercises.FindAsync(entryId);
            if (entry != null)
            {
                _db.ProgramExercises.Remove(entry);
                await _db.SaveChangesAsync();
            }
        }
    }
}