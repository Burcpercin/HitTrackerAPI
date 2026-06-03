using HitTrackerAPI.Models;
using HitTrackerAPI.Repositories;

namespace HitTrackerAPI.Services
{
    public class ProgramService : IProgramService
    {
        private readonly IProgramRepository _repo;

        public ProgramService(IProgramRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<WorkoutProgram>> GetAllAsync(int userId)
            => await _repo.GetAllAsync(userId);

        public async Task<WorkoutProgram?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task<WorkoutProgram> CreateAsync(int userId, string name,
            string goal, int daysPerWeek)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
                throw new Exception("Program name must be at least 3 characters");

            if (daysPerWeek < 1 || daysPerWeek > 7)
                throw new Exception("Days per week must be between 1 and 7");

            var program = new WorkoutProgram
            {
                UserId      = userId,
                Name        = name,
                Goal        = goal,
                DaysPerWeek = daysPerWeek,
                IsActive    = false   // DEFAULT FALSE — iki aktif program bug'ı önlenir
            };

            return await _repo.CreateAsync(program);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var program = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Program not found");

            if (program.UserId != userId)
                throw new Exception("Not authorized");

            await _repo.DeleteAsync(id);
        }

        public async Task SetActiveAsync(int id, int userId)
        {
            var program = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Program not found");

            if (program.UserId != userId)
                throw new Exception("Not authorized");

            // Transaction ile — önce hepsini pasifleştir
            await _repo.SetActiveAsync(id, userId);
        }

        public async Task<ProgramExercise> AddExerciseAsync(int programId,
            int exerciseId, int dayOfWeek, int sets, int targetReps)
        {
            if (sets < 1)    throw new Exception("Sets must be at least 1");
            if (targetReps < 1) throw new Exception("Target reps must be at least 1");

            var pe = new ProgramExercise
            {
                ProgramId  = programId,
                ExerciseId = exerciseId,
                DayOfWeek  = dayOfWeek,
                Sets       = sets,
                TargetReps = targetReps
            };

            return await _repo.AddExerciseAsync(pe);
        }

        public async Task RemoveExerciseAsync(int programId, int entryId)
            => await _repo.RemoveExerciseAsync(entryId);
    }
}