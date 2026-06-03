using Microsoft.AspNetCore.Mvc;
using HitTrackerAPI.Services;

namespace HitTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public record RegisterRequest(string Username, string Email, string Password);
        public record LoginRequest(string Email, string Password);

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || req.Username.Length < 3)
                return BadRequest(new { error = "Username min 3 chars" });

            if (string.IsNullOrWhiteSpace(req.Email) || !req.Email.Contains("@"))
                return BadRequest(new { error = "Invalid email" });

            if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
                return BadRequest(new { error = "Password min 6 chars" });

            try
            {
                var (user, token) = await _authService.RegisterAsync(
                    req.Username, req.Email, req.Password);

                return Created("", new
                {
                    token,
                    user = new { user.Id, user.Username, user.Email }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Please fill in all fields" });

            try
            {
                var (user, token) = await _authService.LoginAsync(req.Email, req.Password);

                return Ok(new
                {
                    token,
                    user = new { user.Id, user.Username, user.Email }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}