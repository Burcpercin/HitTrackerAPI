using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HitTrackerAPI.Services;
using HitTrackerAPI.DTOs;

namespace HitTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AIController : ControllerBase
    {
        private readonly IGeminiService _gemini;
        private readonly IProgramService _programService;

        public AIController(IGeminiService gemini, IProgramService programService)
        {
            _gemini         = gemini;
            _programService = programService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst("userId")!.Value);

        public record WorkoutSuggestionRequest(
            string Goal,
            string ExperienceLevel,
            string Equipment,
            int DaysPerWeek,
            string? Injuries
        );

        /// <summary>Get personalized HIT workout plan from AI</summary>
        [HttpPost("workout-suggestion")]
        public async Task<IActionResult> GetSuggestion(
            [FromBody] WorkoutSuggestionRequest req)
        {
            try
            {
                var suggestion = await _gemini.GenerateWorkoutSuggestionAsync(
                    req.Goal, req.ExperienceLevel,
                    req.Equipment, req.DaysPerWeek, req.Injuries);

                return Ok(new { suggestion });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Save AI suggestion as a workout program</summary>
        [HttpPost("workout-suggestion/save")]
        public async Task<IActionResult> SaveSuggestion(
            [FromBody] CreateProgramRequest req)
        {
            try
            {
                var program = await _programService.CreateAsync(
                    GetUserId(), req.Name, req.Goal, req.DaysPerWeek);

                return Created("", new { program });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}