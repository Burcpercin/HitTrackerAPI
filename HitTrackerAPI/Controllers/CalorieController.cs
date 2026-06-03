using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HitTrackerAPI.Services;
using HitTrackerAPI.Repositories;

namespace HitTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/calories")]
    [Authorize]
    public class CalorieController : ControllerBase
    {
        private readonly ICalorieService _calorieService;
        private readonly IUserRepository _userRepo;

        public CalorieController(
            ICalorieService calorieService,
            IUserRepository userRepo)
        {
            _calorieService = calorieService;
            _userRepo       = userRepo;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst("userId")!.Value);

        public record ProfileRequest(
            DateTime BirthDate,
            float WeightKg,
            float HeightCm,
            string Gender,
            string ActivityLevel
        );

        [HttpPost("profile")]
        public async Task<IActionResult> SaveProfile([FromBody] ProfileRequest req)
        {
            try
            {
                var profile = new Models.UserProfile
                {
                    UserId        = GetUserId(),
                    BirthDate     = req.BirthDate,
                    WeightKg      = req.WeightKg,
                    HeightCm      = req.HeightCm,
                    Gender        = req.Gender,
                    ActivityLevel = req.ActivityLevel
                };

                await _userRepo.SaveProfileAsync(profile);
                return Ok(new { message = "Profile saved" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] string goal = "strength")
        {
            var profile = await _userRepo.GetProfileAsync(GetUserId());
            if (profile == null)
                return NotFound(new { error = "Profile not found" });

            // Yaş hesapla
            var today = DateTime.Today;
            var age   = today.Year - profile.BirthDate.Year;
            if (profile.BirthDate.Date > today.AddYears(-age)) age--;

            var report = _calorieService.Calculate(
                profile.WeightKg,
                profile.HeightCm,
                age,
                profile.Gender,
                profile.ActivityLevel,
                goal
            );

            return Ok(new { profile, report });
        }
    }
}