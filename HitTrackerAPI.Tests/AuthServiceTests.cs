using Moq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using HitTrackerAPI.Models;
using HitTrackerAPI.Repositories;
using HitTrackerAPI.Services;

namespace HitTrackerAPI.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();

            // Test için config
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Secret"]        = "test-secret-key-minimum-32-characters-long",
                    ["Jwt:ExpiresInDays"] = "7"
                })
                .Build();

            _service = new AuthService(_mockRepo.Object, config);
        }

        [Fact]
        public async Task Register_ThrowsException_WhenEmailExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByEmailAsync("test@test.com"))
                     .ReturnsAsync(new User { Email = "test@test.com" });

            // Act
            var act = async () => await _service.RegisterAsync(
                "testuser", "test@test.com", "password123");

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Email already in use*");
        }

        [Fact]
        public async Task Register_ThrowsException_WhenUsernameExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((User?)null);

            _mockRepo.Setup(r => r.GetByUsernameAsync("testuser"))
                     .ReturnsAsync(new User { Username = "testuser" });

            // Act
            var act = async () => await _service.RegisterAsync(
                "testuser", "new@test.com", "password123");

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Username already taken*");
        }

        [Fact]
        public async Task Register_Succeeds_WithValidData()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((User?)null);

            _mockRepo.Setup(r => r.GetByUsernameAsync(It.IsAny<string>()))
                     .ReturnsAsync((User?)null);

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
                     .ReturnsAsync((User u) => { u.Id = 1; return u; });

            // Act
            var (user, token) = await _service.RegisterAsync(
                "newuser", "new@test.com", "password123");

            // Assert
            user.Username.Should().Be("newuser");
            user.Email.Should().Be("new@test.com");
            token.Should().NotBeNullOrEmpty();
            // Şifre hash'lenmiş olmalı
            user.PasswordHash.Should().NotBe("password123");
        }

        [Fact]
        public async Task Login_ThrowsException_WhenEmailNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync((User?)null);

            // Act
            var act = async () => await _service.LoginAsync(
                "wrong@test.com", "password123");

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Invalid email or password*");
        }

        [Fact]
        public async Task Login_ThrowsException_WhenPasswordWrong()
        {
            // Arrange
            var user = new User
            {
                Email        = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };

            _mockRepo.Setup(r => r.GetByEmailAsync("test@test.com"))
                     .ReturnsAsync(user);

            // Act
            var act = async () => await _service.LoginAsync(
                "test@test.com", "wrongpassword");

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Invalid email or password*");
        }

        [Fact]
        public async Task GenerateToken_ReturnsValidToken()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser" };

            // Act
            var token = _service.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Should().HaveCount(3); // JWT = 3 parça
        }
    }
}