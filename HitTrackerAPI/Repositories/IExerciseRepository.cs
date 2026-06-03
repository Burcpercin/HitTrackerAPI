using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public interface IExerciseRepository
    {
        Task<IEnumerable<Exercise>> GetAllAsync(int userId);
        Task<Exercise?> GetByIdAsync(int id);
        Task<Exercise> CreateAsync(Exercise exercise);
        Task<Exercise> UpdateAsync(Exercise exercise);
        Task DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
    }
}