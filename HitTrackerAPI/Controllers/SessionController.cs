using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HitTrackerAPI.Services;
using HitTrackerAPI.DTOs;

namespace HitTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _service;

        public SessionController(ISessionService service)
        {
            _service = service;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst("userId")!.Value);

        /// <summary>Get all sessions for logged in user</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sessions = await _service.GetAllAsync(GetUserId());
            return Ok(sessions);
        }

        /// <summary>Get session by ID with all exercises</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var session = await _service.GetByIdAsync(id);
            if (session == null)
                return NotFound(new { error = "Session not found" });
            return Ok(session);
        }

        /// <summary>Create a new workout session</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSessionRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            try
            {
                var session = await _service.CreateAsync(
                    GetUserId(), req.SessionDate, req.ProgramId);

                return Created("", session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Delete a session</summary>
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

        /// <summary>Log an exercise to a session</summary>
        [HttpPost("{id}/exercises")]
        public async Task<IActionResult> LogExercise(
            int id, [FromBody] LogExerciseRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            try
            {
                var entry = await _service.LogExerciseAsync(
                    id, req.ExerciseId, req.WeightKg,
                    req.Reps, req.ReachedFailure);

                return Created("", entry);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}