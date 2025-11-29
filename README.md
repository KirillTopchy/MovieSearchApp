# MovieSearchApp

A simple two-project solution for searching movies via OMDb API with a Blazor Server frontend and an ASP.NET Core Web API backend. It supports searching by title and stores search history in a local database.

## Projects
- `MovieSearchBackend` – ASP.NET Core Web API (NET 8) that proxies OMDb requests and persists search history using EF Core.
- `MovieSearchFrontend` – Blazor Server (NET 8) UI for searching movies and viewing history.

## Prerequisites
- .NET 8 SDK
- An OMDb API key (https://www.omdbapi.com/apikey.aspx)

## Configuration
1. Backend app settings:
   - Open `MovieSearchBackend/appsettings.Development.json` (or `appsettings.json`).
   - Set `Omdb:ApiKey` to your OMDb API key.
   - Optionally adjust `ConnectionStrings:DefaultConnection` if you want a custom DB location.

Example:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  },
  "Omdb": {
    "BaseUrl": "https://www.omdbapi.com/",
    "ApiKey": "YOUR_KEY"
  }
}
```

## Run locally
You can run backend and frontend from the solution directory.

### Option A: Using dotnet CLI (two terminals)
1. Backend:
   - `cd MovieSearchBackend`
   - `dotnet restore`
   - `dotnet ef database update` (creates the local DB; optional if migrations run on startup)
   - `dotnet run`
   - The API will listen based on `Properties/launchSettings.json` (typically `https://localhost:7245` and `http://localhost:5245`).

2. Frontend:
   - `cd MovieSearchFrontend`
   - `dotnet restore`
   - Ensure frontend is configured to call the backend (by default it uses relative paths like `/api/movies/...` when both run under the same IIS Express/Kestrel solution).
   - `dotnet run`
   - The frontend will start (typically `https://localhost:7108` and `http://localhost:5108`).

### Option B: Start from Visual Studio
- Open the solution, set `MovieSearchBackend` as the startup project and run.
- Then run `MovieSearchFrontend`. If needed, use multiple startup projects.

## Usage
- Open the frontend URL.
- Enter a movie title and press `Search`.
- Click a result to view details.
- Search history appears on the homepage.

## Tests
- Project `MovieSearchBackend.Tests` contains unit tests.
- Run: `cd MovieSearchBackend.Tests && dotnet test`.

## Technical notes
- Target framework: .NET 8; C# 12.
- Backend:
  - Controllers: `MovieSearchBackend/Controllers/MoviesController.cs`.
  - Services: `MovieSearchBackend/Services/MovieService.cs`, `OmdbService.cs`.
  - Options: `MovieSearchBackend/Services/Options/OmdbOptions.cs`.
  - Persistence: EF Core with `AppDbContext` and migrations under `MovieSearchBackend/Migrations`.
  - Middleware for consistent error handling: `Middleware/ExceptionHandlingMiddleware.cs`.
- Frontend:
  - Blazor Server pages under `MovieSearchFrontend/Pages` (e.g., `Index.razor`, `MovieDetails.razor`).
  - Models mirror backend DTOs for deserialization.
- API endpoints (default):
  - `GET /api/movies/search?query={title}`
  - `GET /api/movies/{imdbId}`
  - `GET /api/movies/history`
- OMDb usage: The backend calls OMDb with your API key; the frontend calls backend endpoints.
- Local DB: SQLite (via `Data Source=app.db`) by default.

## Troubleshooting
- Missing OMDb key: Backend returns an error; set `Omdb:ApiKey`.
- CORS/URL mismatch: If hosting separately, ensure frontend calls the correct backend base address or reverse proxy them together.
- SSL dev cert issues: Run `dotnet dev-certs https --trust`.
- Migrations: If schema changes, run `dotnet ef migrations add <Name>` then `dotnet ef database update`.