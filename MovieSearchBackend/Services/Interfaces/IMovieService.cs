using MovieSearchBackend.Domain.Entities;
using MovieSearchBackend.Models;

namespace MovieSearchBackend.Services.Interfaces;

public interface IMovieService
{
    Task<SearchResponse> SearchAsync(string title, int page = 1);
    Task<MovieDetail> GetByIdAsync(string id);
    Task<IReadOnlyCollection<SearchHistory>> GetHistoryAsync();
}
