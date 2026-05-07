var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseRouting();

app.MapGet("/api/time", () =>
{
    return TypedResults.Ok(new
    {
        currentTime = DateTimeOffset.Now.ToString("HH:mm:ss")
    });
});

app.MapRazorPages();

app.Run();