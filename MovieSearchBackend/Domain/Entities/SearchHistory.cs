using System.ComponentModel.DataAnnotations;

namespace MovieSearchBackend.Domain.Entities;

public class SearchHistory
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Query { get; set; } = string.Empty;
    public DateTime SearchedAt { get; set; }
}
