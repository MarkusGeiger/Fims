using System.Security.Claims;
using System.Text.RegularExpressions;
using Fims.Server;
using Fims.Server.Data;
using Fims.Server.Data.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Identity
builder.Services.Configure<ApplicationIdentityOptions>(
  builder.Configuration.GetSection(ApplicationIdentityOptions.Section));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlite(connectionString));
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//
// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//   .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
  .AddRoles<ApplicationRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddTransient<IdentityInitialisation>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// ### AutoMigrate Begin
app.Logger.LogInformation("Starting up. " +
                          $"Working directory: '{Directory.GetCurrentDirectory()}', " +
                          $"Base directory: '{AppContext.BaseDirectory}', " +
                          $"Connection string: '{connectionString}'");
var match = Regex.Match(connectionString, "^DataSource=(?<dbSource>.*);.*$");
var dbPath = match.Groups["dbSource"].Value;
app.Logger.LogInformation($"Before Migration: Database path: '{Path.GetFullPath(dbPath)}', DB exists: {File.Exists(dbPath)}");

// Do the database migrations on startup
using(var scope = app.Services.CreateScope()){
  var init = scope.ServiceProvider.GetService<IdentityInitialisation>();
  if (init != null)
  {
    await init.Run();
  }
}
app.Logger.LogInformation($"After Migration: Database path: '{Path.GetFullPath(dbPath)}', DB exists: {File.Exists(dbPath)}");
// ### AutoMigrate Done


app.UseDefaultFiles();
app.UseStaticFiles();

// Identity API
app.MapGroup("/api").MapIdentityApi<ApplicationUser>();
app.MapPost("/api/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
  await signInManager.SignOutAsync();
  return Results.Ok();
}).RequireAuthorization();
app.MapGet("/api/pingauth", (ClaimsPrincipal user) =>
{
  // This is used by the frontend to acquire information about the logged in user,
  // that's stored inside the HTTP-only cookie in the browser and not accessible from JS
  var email = user.FindFirstValue(ClaimTypes.Email);
  return Results.Json(new { Email = email });
}).RequireAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();