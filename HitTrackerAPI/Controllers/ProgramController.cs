using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HitTrackerAPI.Services;

namespace HitTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/programs")]
    [Authorize]
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService _service;

        public ProgramController(IProgramService service)
        {
            _service = service;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst("userId")!.Value);

        public record CreateProgramRequest(
            string Name,
            string Goal,
            int DaysPerWeek
        );

        public record AddExerciseRequest(
            int ExerciseId,
            int DayOfWeek,
            int Sets,
            int TargetReps
        );

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var programs = await _service.GetAllAsync(GetUserId());
            return Ok(programs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var program = await _service.GetByIdAsync(id);
            if (program == null) return NotFound(new { error = "Program not found" });
            return Ok(program);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProgramRequest req)
        {
            try
            {
                var program = await _service.CreateAsync(
                    GetUserId(), req.Name, req.Goal, req.DaysPerWeek);

                return Created("", program);
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

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                await _service.SetActiveAsync(id, GetUserId());
                return Ok(new { message = "Program activated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/exercises")]
        public async Task<IActionResult> AddExercise(
            int id, [FromBody] AddExerciseRequest req)
        {
            try
            {
                var entry = await _service.AddExerciseAsync(
                    id, req.ExerciseId, req.DayOfWeek, req.Sets, req.TargetReps);

                return Created("", entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}/exercises/{entryId}")]
        public async Task<IActionResult> RemoveExercise(int id, int entryId)
        {
            try
            {
                await _service.RemoveExerciseAsync(id, entryId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}