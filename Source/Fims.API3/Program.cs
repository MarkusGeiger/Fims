var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Text("""
                                   <html>
                                   <body style="font-family:sans-serif;background-color:#363533;color:white;">
                                   <h1>Fims API2</h1>
                                   <a target="_blank" rel="noopener noreferrer" href="https://youtu.be/0uSwPdYOm9k">Raw Coding - ASP.NET Core OAuth Authorization (.NET 7 Minimal Apis C#) (Youtube)</a>
                                   <p>This tutorial shows how to use OAuth with ASP .NET Core APIs.</p>
                                   <p>The authentication is done against the YouTube OAuth.</p>
                                   </body>
                                   </html>
                                   """, "text/html"));

var summaries = new[]
{
  "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
  {
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          summaries[Random.Shared.Next(summaries.Length)]
        ))
      .ToArray();
    return forecast;
  })
  .WithName("GetWeatherForecast")
  .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}