using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClockWeb.Pages;

public class IndexModel : PageModel
{
    public string InitialTime { get; private set; } = string.Empty;

    public void OnGet()
    {
        InitialTime = DateTimeOffset.Now.ToString("HH:mm:ss");
    }
}