using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HitTrackerAPI.Services;

namespace HitTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/exercises")]
    [Authorize]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _service;

        public ExerciseController(IExerciseService service)
        {
            _service = service;
        }

        // Token'dan kullanıcı ID'sini al
        private int GetUserId() =>
            int.Parse(User.FindFirst("userId")!.Value);

        public record CreateExerciseRequest(
            string Name,
            string MuscleGroup,
            string? Description,
            int RequiredRestDays,
            string? Equipment
        );

        public record UpdateExerciseRequest(
            string Name,
            string MuscleGroup,
            string? Description,
            int RequiredRestDays
        );

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _service.GetAllAsync(GetUserId());
            return Ok(exercises);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exercise = await _service.GetByIdAsync(id);
            if (exercise == null) return NotFound(new { error = "Exercise not found" });
            return Ok(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExerciseRequest req)
        {
            try
            {
                var exercise = await _service.CreateAsync(
                    GetUserId(), req.Name, req.MuscleGroup,
                    req.Description, req.RequiredRestDays, req.Equipment);

                return Created("", exercise);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExerciseRequest req)
        {
            try
            {
                var exercise = await _service.UpdateAsync(
                    id, GetUserId(), req.Name, req.MuscleGroup,
                    req.Description, req.RequiredRestDays);

                return Ok(exercise);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id, GetUserId());
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}