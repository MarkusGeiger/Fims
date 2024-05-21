using System.Security.Claims;
using System.Text.RegularExpressions;
using Fims.Server;
using Fims.Server.Data;
using Fims.Server.Data.Migrations;
using Fims.Server.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddIdentity();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.Logger.LogInformation("Starting up. " +
                          $"Working directory: '{Directory.GetCurrentDirectory()}', " +
                          $"Base directory: '{AppContext.BaseDirectory}'");

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

app.Run();