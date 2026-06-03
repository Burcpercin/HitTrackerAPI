using HitTrackerAPI.Models;

namespace HitTrackerAPI.Services
{
    public interface IAuthService
    {
        Task<(User user, string token)> RegisterAsync(string username, string email, string password);
        Task<(User user, string token)> LoginAsync(string email, string password);
        string GenerateToken(User user);
    }
}