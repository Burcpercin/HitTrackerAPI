using Microsoft.EntityFrameworkCore;
using HitTrackerAPI.Data;
using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User> CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<UserProfile?> GetProfileAsync(int userId)
        {
            return await _db.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<UserProfile> SaveProfileAsync(UserProfile profile)
        {
            var existing = await _db.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == profile.UserId);

            if (existing == null)
            {
                _db.UserProfiles.Add(profile);
            }
            else
            {
                existing.WeightKg      = profile.WeightKg;
                existing.HeightCm      = profile.HeightCm;
                existing.BirthDate     = profile.BirthDate;
                existing.Gender        = profile.Gender;
                existing.ActivityLevel = profile.ActivityLevel;
                _db.UserProfiles.Update(existing);
            }

            await _db.SaveChangesAsync();
            return profile;
        }
    }
}