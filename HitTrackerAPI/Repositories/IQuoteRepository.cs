using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public interface IQuoteRepository
    {
        Task<Quote?> GetRandomAsync();
        Task<IEnumerable<Quote>> GetAllAsync();
        Task<Quote> CreateAsync(Quote quote);
        Task SeedAsync();
    }
}