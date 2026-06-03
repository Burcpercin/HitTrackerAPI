using Microsoft.AspNetCore.Mvc;
using HitTrackerAPI.Repositories;

namespace HitTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/quotes")]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteRepository _repo;

        public QuoteController(IQuoteRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandom()
        {
            var quote = await _repo.GetRandomAsync();
            if (quote == null)
                return NotFound(new { error = "No quotes found" });

            return Ok(new
            {
                quote  = quote.Text,
                author = quote.Author,
                category = quote.Category
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var quotes = await _repo.GetAllAsync();
            return Ok(quotes);
        }
    }
}