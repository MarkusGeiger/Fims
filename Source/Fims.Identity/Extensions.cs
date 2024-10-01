using Fims.Identity.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fims.Identity;

public static class Extensions
{
  public static void AddIdentity(this IHostApplicationBuilder builder)
  {
    // Add Identity
    builder.Services.Configure<Options>(
      builder.Configuration.GetSection(Options.Section));
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                           throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<IdentityDbContext>(options =>
      options.UseSqlite(connectionString));

    builder.Services.AddAuthorization();
    builder.Services.AddIdentityApiEndpoints<User>()
      .AddRoles<Role>()
      .AddEntityFrameworkStores<IdentityDbContext>();

    // Add services to the container.
    builder.Services.AddTransient<Initialisation>();
  }
  public static async Task MapIdentityAsync(this WebApplication app)
  {
    // Do the database migrations on startup
    using(var scope = app.Services.CreateScope()){
      var init = scope.ServiceProvider.GetService<Initialisation>();
      if (init != null)
      {
        await init.Run();
      }
    }
    // ### AutoMigrate Done

    app.UseAuthorization();
    
    // Identity API
    app.MapGroup("/api").MapIdentityApi<User>();
  }
}