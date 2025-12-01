namespace MovieSearchFrontend.Services;

using MovieSearchFrontend.Models;

public class SearchState
{
    public string LastQuery { get; set; } = string.Empty;
    public List<MovieSummary> MoviesSummaries { get; set; } = [];
    public bool SearchPerformed { get; set; }
    public List<SearchHistory> SearchHistory { get; set; } = [];
}
