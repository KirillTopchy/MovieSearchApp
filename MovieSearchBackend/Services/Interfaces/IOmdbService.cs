using MovieSearchBackend.Models;

namespace MovieSearchBackend.Services.Interfaces;

public interface IOmdbService
{
    Task<SearchResponse> SearchByTitleAsync(string title);
    Task<MovieDetail> GetByIdAsync(string id);
}
