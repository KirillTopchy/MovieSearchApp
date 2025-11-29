using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MovieSearchFrontend.Pages;

//[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
