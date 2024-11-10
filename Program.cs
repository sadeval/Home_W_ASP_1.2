using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

app.MapPost("/api/data", async (HttpContext context) =>
{
    try
    {
        var data = await context.Request.ReadFromJsonAsync<DataRequest>();

        if (data == null || string.IsNullOrWhiteSpace(data.Message))
        {
            context.Response.StatusCode = 400; 
            await context.Response.WriteAsync("Invalid request. 'message' is required.");
            return;
        }

        DataStorage.Messages.Add(data.Message);

        context.Response.Redirect("/display.html");
    }
    catch (Exception)
    {
        context.Response.StatusCode = 500; 
        await context.Response.WriteAsync("An error occurred processing your request.");
    }
});

app.MapGet("/display.html", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/display.html");
});

app.MapGet("/api/data/messages", async context =>
{
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(DataStorage.Messages);
});

app.Run();

public class DataRequest
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

public static class DataStorage
{
    public static List<string> Messages { get; set; } = new List<string>();
}
