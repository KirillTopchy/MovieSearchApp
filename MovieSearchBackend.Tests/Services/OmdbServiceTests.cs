using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using MovieSearchBackend.Exceptions;
using MovieSearchBackend.Services;
using MovieSearchBackend.Services.Options;
using System.Net;
using Xunit;

namespace MovieSearchBackend.Tests.Services;

public class OmdbServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly OmdbService _omdbService;

    public OmdbServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _omdbService = new OmdbService(_httpClient, Options.Create(new OmdbOptions { ApiKey = "apiKey" }));
    }

    [Fact]
    public async Task SearchByTitleAsync_ReturnsParsedResults()
    {
        // Arrange
        var json = "{\"Search\":[{\"Title\":\"Matrix\",\"Year\":\"1999\",\"imdbID\":\"tt0133093\",\"Type\":\"movie\",\"Poster\":\"N/A\"}],\"totalResults\":\"1\",\"Response\":\"True\"}";
        SetupResponse(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });

        // Act
        var res = await _omdbService.SearchByTitleAsync("Matrix");

        // Assert
        res.Response.Should().BeTrue();
        res.Search.Should().ContainSingle();
        res.TotalResults.Should().Be(1);

        var first = res.Search[0];
        first.Title.Should().Be("Matrix");
        first.Year.Should().Be("1999");
        first.ImdbId.Should().Be("tt0133093");
        first.Type.Should().Be("movie");
        first.Poster.Should().Be("N/A");
    }

    [Fact]
    public async Task SearchByTitleAsync_HandlesMovieNotFoundGracefully()
    {
        // Arrange
        var json = "{\"Search\":[],\"totalResults\":\"0\",\"Response\":\"False\",\"Error\":\"Movie not found!\"}";
        SetupResponse(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });

        // Act
        var res = await _omdbService.SearchByTitleAsync("Unknown");

        // Assert
        res.Response.Should().BeTrue();
        res.Search.Should().BeEmpty();
        res.TotalResults.Should().Be(0);
    }

    [Fact]
    public async Task SearchByTitleAsync_ThrowsNotFoundForOtherError()
    {
        // Arrange
        var json = "{\"Search\":[],\"totalResults\":\"0\",\"Response\":\"False\",\"Error\":\"Some other error\"}";
        SetupResponse(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _omdbService.SearchByTitleAsync("Unknown"));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsParsedMovie()
    {
        // Arrange
        var json = "{\"Title\":\"Matrix\",\"Year\":\"1999\",\"imdbID\":\"tt0133093\",\"Type\":\"movie\",\"Poster\":\"N/A\",\"Response\":\"True\"}";
        SetupResponse(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });

        // Act
        var res = await _omdbService.GetByIdAsync("tt0133093");

        // Assert
        res.Response.Should().BeTrue();
        res.Title.Should().Be("Matrix");
        res.Year.Should().Be("1999");
        res.ImdbId.Should().Be("tt0133093");
        res.Type.Should().Be("movie");
        res.Poster.Should().Be("N/A");
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundWhenResponseFalse()
    {
        // Arrange
        var json = "{\"Response\":\"False\",\"Error\":\"Not found\"}";
        SetupResponse(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _omdbService.GetByIdAsync("bad"));
    }

    [Fact]
    public async Task FetchAsync_ThrowsExternalServiceException_OnHttpError()
    {
        // Arrange
        SetupResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("err") });

        // Act & Assert
        await Assert.ThrowsAsync<ExternalServiceException>(() => _omdbService.GetByIdAsync("tt123"));
    }

    private void SetupResponse(HttpResponseMessage response)
    {
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }
}
