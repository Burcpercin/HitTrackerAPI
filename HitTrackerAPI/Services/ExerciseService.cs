using HitTrackerAPI.Models;
using HitTrackerAPI.Repositories;

namespace HitTrackerAPI.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _repo;

        public ExerciseService(IExerciseRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync(int userId)
            => await _repo.GetAllAsync(userId);

        public async Task<Exercise?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task<Exercise> CreateAsync(int userId, string name,
            string muscleGroup, string? description,
            int requiredRestDays, string? equipment)
        {
            // Mentzer kuralı — minimum 3 gün dinlenme
            if (requiredRestDays < 3)
                throw new Exception("Minimum 3 rest days required (Mentzer principle)");

            if (await _repo.ExistsByNameAsync(name))
                throw new Exception("Exercise with this name already exists");

            var exercise = new Exercise
            {
                Name             = name,
                MuscleGroup      = muscleGroup,
                Description      = description,
                RequiredRestDays = requiredRestDays,
                Equipment        = equipment,
                IsCustom         = true,
                UserId           = userId
            };

            return await _repo.CreateAsync(exercise);
        }

        public async Task<Exercise> UpdateAsync(int id, int userId, string name,
            string muscleGroup, string? description, int requiredRestDays)
        {
            var exercise = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Exercise not found");

            // Sadece sahibi düzenleyebilir
            if (!exercise.IsCustom || exercise.UserId != userId)
                throw new Exception("You can only edit your own exercises");

            if (requiredRestDays < 3)
                throw new Exception("Minimum 3 rest days required");

            exercise.Name             = name;
            exercise.MuscleGroup      = muscleGroup;
            exercise.Description      = description;
            exercise.RequiredRestDays = requiredRestDays;

            return await _repo.UpdateAsync(exercise);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var exercise = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Exercise not found");

            if (!exercise.IsCustom || exercise.UserId != userId)
                throw new Exception("You can only delete your own exercises");

            await _repo.DeleteAsync(id);
        }
    }
}