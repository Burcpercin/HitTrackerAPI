using Microsoft.EntityFrameworkCore;
using HitTrackerAPI.Data;
using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly AppDbContext _db;

        public ExerciseRepository(AppDbContext db)
        {
            _db = db;
        }

        // Tüm egzersizleri getir
        // Seed egzersizler herkese görünür, custom sadece sahibine
        public async Task<IEnumerable<Exercise>> GetAllAsync(int userId)
        {
            return await _db.Exercises
                .Where(e => !e.IsCustom || e.UserId == userId)
                .OrderBy(e => e.MuscleGroup)
                .ThenBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Exercise?> GetByIdAsync(int id)
        {
            return await _db.Exercises.FindAsync(id);
        }

        public async Task<Exercise> CreateAsync(Exercise exercise)
        {
            _db.Exercises.Add(exercise);
            await _db.SaveChangesAsync();
            return exercise;
        }

        public async Task<Exercise> UpdateAsync(Exercise exercise)
        {
            _db.Exercises.Update(exercise);
            await _db.SaveChangesAsync();
            return exercise;
        }

        public async Task DeleteAsync(int id)
        {
            var exercise = await _db.Exercises.FindAsync(id);
            if (exercise != null)
            {
                _db.Exercises.Remove(exercise);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Exercises
                .AnyAsync(e => e.Name.ToLower() == name.ToLower());
        }
    }
}