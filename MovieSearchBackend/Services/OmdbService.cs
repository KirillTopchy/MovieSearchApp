using Microsoft.Extensions.Options;
using MovieSearchBackend.Exceptions;
using MovieSearchBackend.Models;
using MovieSearchBackend.Services.Interfaces;
using MovieSearchBackend.Services.Options;
using System.Text.Json;

namespace MovieSearchBackend.Services;

public class OmdbService(HttpClient http, IOptions<OmdbOptions> options) : IOmdbService
{
    private readonly HttpClient _http = http;
    private readonly string _baseUrl = "https://www.omdbapi.com/?apikey=";
    private readonly string _apiKey = options?.Value?.ApiKey ?? throw new ArgumentException("Omdb:ApiKey is not configured");
    private const string MovieNotFoundMessage = "Movie not found!";

    public async Task<SearchResponse> SearchByTitleAsync(string title)
    {
        var url = $"{_baseUrl}{_apiKey}&s={Uri.EscapeDataString(title)}";
        var content = await FetchAsync(url);
        var doc = Deserialize<SearchResponse>(content);

        if (!doc.Response)
        {
            if (string.Equals(doc.Error, MovieNotFoundMessage, StringComparison.OrdinalIgnoreCase))
            {
                return new SearchResponse { Search = [], TotalResults = 0, Response = true };
            }
            throw new NotFoundException(doc.Error ?? "No results");
        }

        return doc;
    }

    public async Task<MovieDetail> GetByIdAsync(string id)
    {
        var url = $"{_baseUrl}{_apiKey}&i={Uri.EscapeDataString(id)}&plot=full";
        var content = await FetchAsync(url);
        var doc = Deserialize<MovieDetail>(content);

        if (!doc.Response)
        {
            throw new NotFoundException(doc.Error ?? "Not found");
        }

        return doc;
    }

    private async Task<string> FetchAsync(string url)
    {
        HttpResponseMessage res;
        try { res = await _http.GetAsync(url); }
        catch (Exception ex) { throw new ExternalServiceException("Failed to contact OMDB service: " + ex.Message); }

        var content = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
        {
            throw new ExternalServiceException($"OMDB returned status {(int)res.StatusCode}: {res.ReasonPhrase}");
        }

        return content;
    }

    private static T Deserialize<T>(string json) where T : class
    {
        var doc = JsonSerializer.Deserialize<T>(json);
        return doc ?? throw new ExternalServiceException("Failed to parse OMDB response");
    }
}