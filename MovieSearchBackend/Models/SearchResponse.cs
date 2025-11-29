using MovieSearchBackend.Models.TypeConverters;
using System.Text.Json.Serialization;

namespace MovieSearchBackend.Models;

public class SearchResponse
{
    public List<MovieSummary> Search { get; set; } = [];

    [JsonPropertyName("totalResults")]
    [JsonConverter(typeof(IntegerStringConverter))]
    public int TotalResults { get; set; }

    [JsonPropertyName("Response")]
    [JsonConverter(typeof(BooleanStringConverter))]
    public bool Response { get; set; }

    public string? Error { get; set; }
}