using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
        Task<UserProfile?> GetProfileAsync(int userId);
        Task<UserProfile> SaveProfileAsync(UserProfile profile);
    }
}