using Microsoft.EntityFrameworkCore;
using HitTrackerAPI.Data;
using HitTrackerAPI.Models;

namespace HitTrackerAPI.Repositories
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly AppDbContext _db;

        public QuoteRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Quote?> GetRandomAsync()
        {
            var count = await _db.Quotes.CountAsync();
            if (count == 0) return null;
            var skip = new Random().Next(0, count);
            return await _db.Quotes.Skip(skip).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Quote>> GetAllAsync()
            => await _db.Quotes.ToListAsync();

        public async Task<Quote> CreateAsync(Quote quote)
        {
            _db.Quotes.Add(quote);
            await _db.SaveChangesAsync();
            return quote;
        }

        public async Task SeedAsync()
        {
            if (await _db.Quotes.AnyAsync()) return;

            var quotes = new List<Quote>
            {
                new() { Text = "One set to failure is worth ten half-hearted sets.", Author = "Mike Mentzer", Category = "training" },
                new() { Text = "Train harder, but train less.", Author = "Mike Mentzer", Category = "training" },
                new() { Text = "The ideal number of sets per exercise is one.", Author = "Mike Mentzer", Category = "training" },
                new() { Text = "High intensity training is the only training.", Author = "Mike Mentzer", Category = "training" },
                new() { Text = "Rest is when growth happens.", Author = "Mike Mentzer", Category = "recovery" },
                new() { Text = "Intensity is the key to growth.", Author = "Mike Mentzer", Category = "training" },
                new() { Text = "Brief, intense, infrequent — that is the formula.", Author = "Mike Mentzer", Category = "training" },
                new() { Text = "Progressive overload is the only way forward.", Author = "Mike Mentzer", Category = "training" },
                new() { Text = "Pain is weakness leaving the body.", Author = "Unknown", Category = "motivation" },
                new() { Text = "Push yourself because no one else is going to do it for you.", Author = "Unknown", Category = "motivation" }
            };

            _db.Quotes.AddRange(quotes);
            await _db.SaveChangesAsync();
        }
    }
}