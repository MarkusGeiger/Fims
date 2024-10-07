using System.Security.Claims;
using Fims.API2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// This sample is created following the tutorial in https://youtu.be/Blrn5JyAl6E

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(o =>
  {
    o.RequireHttpsMetadata = false;
    o.Audience = builder.Configuration["Authentication:Audience"];
    o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
    o.TokenValidationParameters = new TokenValidationParameters
    {
      ValidIssuer = builder.Configuration["Authentication:ValidIssuer"]
    };
  });

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.MapGet("/", () => """
                      <html>
                      <body>
                      <h1>Fims API2</h1>
                      <a href="https://youtu.be/Blrn5JyAl6E"></a>
                      This tutorial shows how to use Keycloak with ASP .NET Core APIs.
                      The authentication is done here with username and password.
                      To obtain a valid token, the user has to send username and password to keycloak.
                      Problem: No cookie is set, login has to be performed on each page load!
                      Only way to persist the cookie is to use local/session storage => unsecure.
                      </body>
                      </html>
                      """);

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

app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>
{
  return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}