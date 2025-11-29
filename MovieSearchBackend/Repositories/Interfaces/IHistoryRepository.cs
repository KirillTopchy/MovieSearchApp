using MovieSearchBackend.Domain.Entities;

namespace MovieSearchBackend.Repositories.Interfaces;

public interface IHistoryRepository
{
    Task AddQueryAsync(string query);
    Task<IReadOnlyCollection<SearchHistory>> GetLastFiveAsync();
}
