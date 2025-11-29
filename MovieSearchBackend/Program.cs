using Microsoft.EntityFrameworkCore;
using MovieSearchBackend.Infrastructure;
using MovieSearchBackend.Middleware.Extensions;
using MovieSearchBackend.Repositories;
using MovieSearchBackend.Repositories.Interfaces;
using MovieSearchBackend.Services;
using MovieSearchBackend.Services.Interfaces;
using MovieSearchBackend.Services.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure sqlite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=history.db"));

// configure Omdb options from configuration
builder.Services.Configure<OmdbOptions>(builder.Configuration.GetSection("Omdb"));

// register services and repositories
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddHttpClient<IOmdbService, OmdbService>();

var app = builder.Build();

// apply EF Core migrations at startup to ensure database/schema are up to date
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// use custom middleware for exception handling
app.UseApiExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();