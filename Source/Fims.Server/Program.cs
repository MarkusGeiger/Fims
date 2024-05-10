using System.Security.Claims;
using Fims.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Identity
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
  .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
