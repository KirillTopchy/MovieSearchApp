using MovieSearchBackend.Domain.Entities;
using MovieSearchBackend.Models;
using MovieSearchBackend.Repositories.Interfaces;
using MovieSearchBackend.Services.Interfaces;

namespace MovieSearchBackend.Services;

public class MovieService(IOmdbService omdb, IHistoryRepository history) : IMovieService
{
    private readonly IOmdbService _omdb = omdb;
    private readonly IHistoryRepository _history = history;

    public async Task<SearchResponse> SearchAsync(string title, int page = 1)
    {
        await _history.AddQueryAsync(title);
        return await _omdb.SearchByTitleAsync(title, page);
    }

    public async Task<MovieDetail> GetByIdAsync(string id)
    {
        return await _omdb.GetByIdAsync(id);
    }

    public async Task<IReadOnlyCollection<SearchHistory>> GetHistoryAsync()
    {
        return await _history.GetLastFiveAsync();
    }
}
