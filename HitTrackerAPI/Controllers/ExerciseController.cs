using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HitTrackerAPI.Services;
using HitTrackerAPI.DTOs;

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

        /// <summary>Get all exercises</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _service.GetAllAsync(GetUserId());
            return Ok(exercises);
        }

        /// <summary>Get exercise by ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exercise = await _service.GetByIdAsync(id);
            if (exercise == null)
                return NotFound(new { error = "Exercise not found" });
            return Ok(exercise);
        }

        /// <summary>Create a custom exercise</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExerciseRequest req)
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

        /// <summary>Update a custom exercise</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExerciseRequest req)
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

        /// <summary>Delete a custom exercise</summary>
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