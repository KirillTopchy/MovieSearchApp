using MovieSearchBackend.Models;

namespace MovieSearchBackend.Services.Interfaces;

public interface IOmdbService
{
    Task<SearchResponse> SearchByTitleAsync(string title, int page = 1);
    Task<MovieDetail> GetByIdAsync(string id);
}
