using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HitTrackerAPI.Models;
using HitTrackerAPI.Repositories;

namespace HitTrackerAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config   = config;
        }

        public async Task<(User user, string token)> RegisterAsync(
            string username, string email, string password)
        {
            // Email ve username kontrolü
            if (await _userRepo.GetByEmailAsync(email) != null)
                throw new Exception("Email already in use");

            if (await _userRepo.GetByUsernameAsync(username) != null)
                throw new Exception("Username already taken");

            // Şifreyi hashle
            var user = new User
            {
                Username     = username,
                Email        = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            var created = await _userRepo.CreateAsync(user);
            var token   = GenerateToken(created);

            return (created, token);
        }

        public async Task<(User user, string token)> LoginAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email)
                ?? throw new Exception("Invalid email or password");

            // Hash karşılaştır
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            var token = GenerateToken(user);
            return (user, token);
        }

        public string GenerateToken(User user)
        {
            var secret  = _config["Jwt:Secret"]!;
            var expires = int.Parse(_config["Jwt:ExpiresInDays"] ?? "7");

            var claims = new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim("username", user.Username)
            };

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims:   claims,
                expires:  DateTime.UtcNow.AddDays(expires),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}