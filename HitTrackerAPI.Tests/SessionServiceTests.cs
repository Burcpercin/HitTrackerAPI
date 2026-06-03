using Moq;
using FluentAssertions;
using HitTrackerAPI.Models;
using HitTrackerAPI.Repositories;
using HitTrackerAPI.Services;

namespace HitTrackerAPI.Tests
{
    public class SessionServiceTests
    {
        private readonly Mock<ISessionRepository> _mockRepo;
        private readonly SessionService _service;

        public SessionServiceTests()
        {
            _mockRepo = new Mock<ISessionRepository>();
            _service  = new SessionService(_mockRepo.Object);
        }

        [Fact]
        public async Task LogExercise_ThrowsException_WhenWeightIsZero()
        {
            // Act
            var act = async () => await _service.LogExerciseAsync(
                1, 1, 0, 8, false);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Weight must be greater than 0*");
        }

        [Fact]
        public async Task LogExercise_ThrowsException_WhenRepsIsZero()
        {
            // Act
            var act = async () => await _service.LogExerciseAsync(
                1, 1, 100, 0, false);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Reps must be greater than 0*");
        }

        [Fact]
        public async Task LogExercise_Succeeds_WithValidData()
        {
            // Arrange
            _mockRepo.Setup(r => r.AddExerciseAsync(It.IsAny<SessionExercise>()))
                     .ReturnsAsync((SessionExercise se) => se);

            // Act
            var result = await _service.LogExerciseAsync(
                1, 1, 100, 8, true);

            // Assert
            result.WeightKg.Should().Be(100);
            result.Reps.Should().Be(8);
            result.ReachedFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_ThrowsException_WhenNotOwner()
        {
            // Arrange
            var session = new WorkoutSession
            {
                Id     = 1,
                UserId = 999  // farklı kullanıcı
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(session);

            // Act
            var act = async () => await _service.DeleteAsync(1, 1);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Not authorized*");
        }
    }
}