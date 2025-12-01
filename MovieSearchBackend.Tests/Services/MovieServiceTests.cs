using FluentAssertions;
using Moq;
using MovieSearchBackend.Domain.Entities;
using MovieSearchBackend.Models;
using MovieSearchBackend.Repositories.Interfaces;
using MovieSearchBackend.Services;
using MovieSearchBackend.Services.Interfaces;
using Xunit;

namespace MovieSearchBackend.Tests.Services;

public class MovieServiceTests
{
    private readonly Mock<IOmdbService> _omdbServiceMock;
    private readonly Mock<IHistoryRepository> _historyRepositoryMock;
    private readonly IMovieService _movieService;

    public MovieServiceTests()
    {
        _omdbServiceMock = new Mock<IOmdbService>();
        _historyRepositoryMock = new Mock<IHistoryRepository>();
        _movieService = new MovieService(_omdbServiceMock.Object, _historyRepositoryMock.Object);
    }

    [Fact]
    public async Task SearchAsync_CallsHistoryAndOmdb()
    {
        // Arrange
        var expectedResponse = new SearchResponse
        {
            Response = true,
            TotalResults = 1,
            Search = [ new MovieSummary { Title = "matrix", Year = "2000", ImdbId = "tt1", Type = "movie", Poster = "N/A" } ]
        };

        _omdbServiceMock.Setup(m => m.SearchByTitleAsync("matrix", 1)).ReturnsAsync(expectedResponse).Verifiable();
        _historyRepositoryMock.Setup(h => h.AddQueryAsync("matrix")).Returns(Task.CompletedTask).Verifiable();

        // Act
        var res = await _movieService.SearchAsync("matrix", 1);


        // Assert
        res.Response.Should().BeTrue();
        res.Search.Should().ContainSingle().Which.Title.Should().Be("matrix");
        _omdbServiceMock.Verify();
        _historyRepositoryMock.Verify();
    }

    [Fact]
    public async Task GetByIdAsync_CallsOmdb()
    {
        // Arrange
        var detail = new MovieDetail { Title = "Test", Response = true };
        _omdbServiceMock.Setup(m => m.GetByIdAsync("tt1")).ReturnsAsync(detail).Verifiable();

        // Act
        var res = await _movieService.GetByIdAsync("tt1");

        // Assert
        res.Response.Should().BeTrue();
        res.Title.Should().Be("Test");
        _omdbServiceMock.Verify();
        _historyRepositoryMock.Verify(h => h.AddQueryAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsRepositoryData()
    {
        // Arrange
        var histories = new List<SearchHistory>
        {
            new() { Query = "a", SearchedAt = DateTime.UtcNow },
            new() { Query = "b", SearchedAt = DateTime.UtcNow }
        } as IReadOnlyCollection<SearchHistory>;

        _historyRepositoryMock.Setup(h => h.GetLastFiveAsync()).ReturnsAsync(histories).Verifiable();

        // Act
        var hist = await _movieService.GetHistoryAsync();

        // Assert
        hist.Count.Should().Be(2);
        hist.Should().Contain(h => h.Query == "a");
        hist.Should().Contain(h => h.Query == "b");
        _historyRepositoryMock.Verify();
    }
}
