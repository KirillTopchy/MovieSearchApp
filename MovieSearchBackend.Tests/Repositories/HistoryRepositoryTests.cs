using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MovieSearchBackend.Infrastructure;
using MovieSearchBackend.Repositories;
using Xunit;

namespace MovieSearchBackend.Tests.Repositories;

public class HistoryRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddQueryAsync_AddsNewQuery()
    {
        using var ctx = CreateContext();
        var repo = new HistoryRepository(ctx);

        await repo.AddQueryAsync("  lord of the rings  ");

        ctx.SearchHistories.Should().ContainSingle();
        var entry = ctx.SearchHistories.First();
        entry.Query.Should().Be("lord of the rings");
        entry.SearchedAt.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public async Task AddQueryAsync_UpdatesExistingQueryTimestamp()
    {
        using var ctx = CreateContext();
        var repo = new HistoryRepository(ctx);
        await repo.AddQueryAsync("test");
        var firstTime = ctx.SearchHistories.First().SearchedAt;
        await Task.Delay(10);
        await repo.AddQueryAsync("test");
        var updatedTime = ctx.SearchHistories.First().SearchedAt;
        updatedTime.Should().BeAfter(firstTime);
        ctx.SearchHistories.Should().ContainSingle();
    }

    [Fact]
    public async Task GetLastFiveAsync_ReturnsFiveMostRecent()
    {
        using var ctx = CreateContext();
        var repo = new HistoryRepository(ctx);
        for (int i = 0; i < 7; i++)
        {
            await repo.AddQueryAsync($"q{i}");
            await Task.Delay(5);
        }

        var lastFive = await repo.GetLastFiveAsync();
        lastFive.Count.Should().Be(5);
        var ordered = lastFive.OrderByDescending(x => x.SearchedAt).ToList();
        lastFive.Should().BeEquivalentTo(ordered, options => options.WithoutStrictOrdering());
    }
}
