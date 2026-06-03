using Moq;
using FluentAssertions;
using HitTrackerAPI.Models;
using HitTrackerAPI.Repositories;
using HitTrackerAPI.Services;

namespace HitTrackerAPI.Tests
{
    public class ExerciseServiceTests
    {
        private readonly Mock<IExerciseRepository> _mockRepo;
        private readonly ExerciseService _service;

        public ExerciseServiceTests()
        {
            _mockRepo = new Mock<IExerciseRepository>();
            _service  = new ExerciseService(_mockRepo.Object);
        }

        // ── Mentzer Kuralı ───────────────────────────────────

        [Fact]
        public async Task Create_ThrowsException_WhenRestDaysLessThan3()
        {
            // Arrange
            var restDays = 2;

            // Act
            var act = async () => await _service.CreateAsync(
                1, "Test Exercise", "Chest", null, restDays, null);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Minimum 3 rest days*");
        }

        [Fact]
        public async Task Create_ThrowsException_WhenExerciseNameExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.ExistsByNameAsync("Bench Press"))
                     .ReturnsAsync(true);

            // Act
            var act = async () => await _service.CreateAsync(
                1, "Bench Press", "Chest", null, 5, null);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*already exists*");
        }

        [Fact]
        public async Task Create_Succeeds_WithValidData()
        {
            // Arrange
            _mockRepo.Setup(r => r.ExistsByNameAsync(It.IsAny<string>()))
                     .ReturnsAsync(false);

            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Exercise>()))
                     .ReturnsAsync((Exercise e) => e);

            // Act
            var result = await _service.CreateAsync(
                1, "Bench Press", "Chest", null, 5, "Barbell");

            // Assert
            result.Name.Should().Be("Bench Press");
            result.IsCustom.Should().BeTrue();
            result.RequiredRestDays.Should().Be(5);
        }

        [Fact]
        public async Task Delete_ThrowsException_WhenNotOwner()
        {
            // Arrange
            var exercise = new Exercise
            {
                Id       = 1,
                IsCustom = true,
                UserId   = 999  // farklı kullanıcı
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(exercise);

            // Act — userId = 1, ama egzersiz userId = 999'a ait
            var act = async () => await _service.DeleteAsync(1, 1);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*only delete your own*");
        }

        [Fact]
        public async Task Delete_ThrowsException_WhenExerciseNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(99))
                     .ReturnsAsync((Exercise?)null);

            // Act
            var act = async () => await _service.DeleteAsync(99, 1);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*not found*");
        }
    }
}