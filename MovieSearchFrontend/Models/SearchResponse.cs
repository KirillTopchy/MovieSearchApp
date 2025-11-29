using MovieSearchFrontend.Models;
using System.Text.Json.Serialization;

namespace MovieSearchFrontend.Models;

public class SearchResponse
{
    public List<MovieSummary> Search { get; set; } = [];
    public string Response { get; set; } = string.Empty;

    [JsonPropertyName("totalResults")]
    public string TotalResults { get; set; } = string.Empty;
}
