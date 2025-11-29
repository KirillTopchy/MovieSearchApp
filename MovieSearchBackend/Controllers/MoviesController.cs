using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using MovieSearchBackend.Domain.Entities;
using MovieSearchBackend.Models;
using MovieSearchBackend.Services.Interfaces;

namespace MovieSearchBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    private readonly IMovieService _movieService = movieService;

    [HttpGet("search")]
    public async Task<ActionResult<SearchResponse>> Search([FromQuery, Required] string query)
    {
        var doc = await _movieService.SearchAsync(query);
        return Ok(doc);
    }

    [HttpGet("details/{id}")]
    public async Task<ActionResult<MovieDetail>> Details([FromRoute, Required] string id)
    {
        var details = await _movieService.GetByIdAsync(id);
        return Ok(details);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IReadOnlyCollection<SearchHistory>>> History()
    {
        var searchHistory = await _movieService.GetHistoryAsync();
        return Ok(searchHistory);
    }
}