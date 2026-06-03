using HitTrackerAPI.Models;

namespace HitTrackerAPI.Services
{
    public interface IExerciseService
    {
        Task<IEnumerable<Exercise>> GetAllAsync(int userId);
        Task<Exercise?> GetByIdAsync(int id);
        Task<Exercise> CreateAsync(int userId, string name, string muscleGroup,
            string? description, int requiredRestDays, string? equipment);
        Task<Exercise> UpdateAsync(int id, int userId, string name, string muscleGroup,
            string? description, int requiredRestDays);
        Task DeleteAsync(int id, int userId);
    }
}