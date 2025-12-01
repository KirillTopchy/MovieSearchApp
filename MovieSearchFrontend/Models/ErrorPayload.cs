namespace MovieSearchFrontend.Models;

public sealed class ErrorPayload
{
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public string? ErrorCode { get; set; }
    public string? TraceId { get; set; }
    public int? Status { get; set; }
}
