using System.Security.Claims;
using Fims.API3;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("cookie")
  .AddCookie("cookie")
  .AddOAuth("keycloak", o =>
  {
    o.SignInScheme = "cookie";
    o.ClientId = Secrets.ClientId;
    o.ClientSecret = Secrets.ClientSecret;
    o.SaveTokens = false;
  });

builder.Services.AddAuthorization(b =>
{
  b.AddPolicy("keycloak-enabled", pb =>
  {
    pb.AddAuthenticationSchemes("cookie")
      .RequireClaim("kc-token", "y")
      .RequireAuthenticatedUser();
  });
});

builder.AddServiceDefaults();

builder.Services.AddSingleton<Database>();
builder.Services.AddHttpClient();

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

app.MapDefaultEndpoints();

app.MapGet("/login", () => Results.SignIn(
  new ClaimsPrincipal(
    new ClaimsIdentity
    (new[] { new Claim("user_id", Guid.NewGuid().ToString()) },
      "cookie"
    )
  ),
  authenticationScheme: "cookie"
));

app.MapGet("/kc/info", (IHttpClientFactory clientFactory, Database db, HttpContext ctx) =>
{
  var user = ctx.User;
  var userId = user.FindFirstValue("user_id");
  var accessToken = db[userId];
  var client = clientFactory.CreateClient();

}).RequireAuthorization("keycloak-enabled");

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

class Database : Dictionary<string, object>
{
  
}