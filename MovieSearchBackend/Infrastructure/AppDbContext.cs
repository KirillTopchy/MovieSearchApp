using Microsoft.EntityFrameworkCore;
using MovieSearchBackend.Domain.Entities;

namespace MovieSearchBackend.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SearchHistory> SearchHistories { get; set; }
}
