using System.Security.Claims;
using Fims.Server.Data;
using Fims.Server.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityInitialisation = Fims.Server.Identity.IdentityInitialisation;

namespace Fims.Server.Identity;

public static class IdentityExtensions
{
  public static void AddIdentity(this WebApplicationBuilder builder)
  {
    // Add Identity
    builder.Services.Configure<ApplicationIdentityOptions>(
      builder.Configuration.GetSection(ApplicationIdentityOptions.Section));
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                           throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlite(connectionString));

    builder.Services.AddAuthorization();
    builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
      .AddRoles<ApplicationRole>()
      .AddEntityFrameworkStores<ApplicationDbContext>();

    // Add services to the container.
    builder.Services.AddTransient<IdentityInitialisation>();
  }
  public static async Task MapIdentityAsync(this WebApplication app)
  {
    // Do the database migrations on startup
    using(var scope = app.Services.CreateScope()){
      var init = scope.ServiceProvider.GetService<IdentityInitialisation>();
      if (init != null)
      {
        await init.Run();
      }
    }
    // ### AutoMigrate Done

    app.UseAuthorization();
    
    // Identity API
    app.MapGroup("/api").MapIdentityApi<ApplicationUser>();
  }
}