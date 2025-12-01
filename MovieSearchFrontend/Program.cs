using MovieSearchFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<SearchState>();

// typed http client
builder.Services.AddHttpClient<MovieApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
