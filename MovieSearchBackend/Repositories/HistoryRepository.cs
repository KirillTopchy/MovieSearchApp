using Microsoft.EntityFrameworkCore;
using MovieSearchBackend.Domain.Entities;
using MovieSearchBackend.Infrastructure;
using MovieSearchBackend.Repositories.Interfaces;

namespace MovieSearchBackend.Repositories;

public class HistoryRepository(AppDbContext db) : IHistoryRepository
{
    private readonly AppDbContext _db = db;

    public async Task AddQueryAsync(string query)
    {
        query = query.Trim();

        var existing = await _db.SearchHistories.FirstOrDefaultAsync(s => s.Query == query);
        if (existing != null)
        {
            existing.SearchedAt = DateTime.UtcNow;
            _db.SearchHistories.Update(existing);
        }
        else
        {
            _db.SearchHistories.Add(new SearchHistory { Query = query, SearchedAt = DateTime.UtcNow });
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<SearchHistory>> GetLastFiveAsync()
    {
        return await _db.SearchHistories.OrderByDescending(s => s.SearchedAt).Take(5).ToListAsync();
    }
}