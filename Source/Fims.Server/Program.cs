using System.Reflection;
using System.Runtime.InteropServices;
using Fims.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddIdentity();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.Logger.LogInformation("Starting up. " +
                          $"Version: {Assembly.GetEntryAssembly()?.GetName().Version}, " +
                          $"CLR Version: {Environment.Version}, " +
                          $"OS Version: {Environment.OSVersion}, " +
                          $"OS: {RuntimeInformation.OSDescription}, " +
                          $"Architecture: {RuntimeInformation.ProcessArchitecture}, " +
                          $"Working directory: '{Directory.GetCurrentDirectory()}', " +
                          $"Base directory: '{AppContext.BaseDirectory}', " +
                          $"Current directory: '{Environment.CurrentDirectory}'");

app.UseDefaultFiles();
app.UseStaticFiles();
await app.MapIdentityAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.MapDefaultEndpoints();

app.Run();