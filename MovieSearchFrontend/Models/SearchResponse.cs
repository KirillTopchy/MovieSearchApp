namespace MovieSearchFrontend.Models;

using System.Text.Json.Serialization;

public class SearchResponse
{
    public List<MovieSummary> Search { get; set; } = [];

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }
}
