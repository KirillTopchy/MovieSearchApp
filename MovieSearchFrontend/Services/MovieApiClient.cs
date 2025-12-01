using MovieSearchFrontend.Models;
using System.Text.Json;

namespace MovieSearchFrontend.Services;

public class MovieApiClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _httpClient = httpClient;

    public async Task<SearchResponse?> SearchAsync(string query, int page = 1, CancellationToken cancellationToken = default)
    {
        return await GetAsync<SearchResponse>($"/api/movies/search?query={Uri.EscapeDataString(query)}&page={page}", cancellationToken);
    }

    public Task<MovieDetail?> GetDetailsAsync(string id, CancellationToken cancellationToken = default)
        => GetAsync<MovieDetail>($"/api/movies/details/{id}", cancellationToken);

    public async Task<List<SearchHistory>> GetHistoryAsync(CancellationToken cancellationToken = default)
    {
        var res = await GetAsync<List<SearchHistory>>("/api/movies/history", cancellationToken);
        return res ?? [];
    }

    private static async Task ThrowForErrorAsync(HttpResponseMessage res)
    {
        var status = (int)res.StatusCode;
        string message = $"Request failed with status {status}";

        var content = await res.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                var problem = JsonSerializer.Deserialize<ErrorPayload>(content, _jsonOptions);
                if (problem != null && !string.IsNullOrWhiteSpace(problem.Title))
                {
                    message = string.IsNullOrWhiteSpace(problem.Detail)
                        ? problem.Title
                        : $"{problem.Title}: {problem.Detail}";
                }
                else
                {
                    message = content;
                }
            }
            catch (JsonException)
            {
                message = content;
            }
        }

        throw new HttpRequestException(message, null, res.StatusCode);
    }

    private async Task<T?> GetAsync<T>(string uri, CancellationToken cancellationToken)
    {
        var res = await _httpClient.GetAsync(uri, cancellationToken);
        
        if (!res.IsSuccessStatusCode)
        {
            await ThrowForErrorAsync(res);
        }

        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }
}
