using System.Text.RegularExpressions;
using Fims.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Options = Fims.Identity.Options;

namespace Fims.Identity;

public partial class Initialisation(
  ILogger<Initialisation> logger,
  IConfiguration configuration,
  IdentityDbContext identityDb,
  UserManager<User> userManager,
  RoleManager<Role> roleManager,
  IOptions<Options> options)
{
  [GeneratedRegex("^DataSource=(?<dbSource>.*);.*$")]
  private static partial Regex ConnectionStringRegex();
  
  public async Task Run()
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      logger.LogCritical("Failed to get connection string 'DefaultConnection'.");
      return;
    }
    logger.LogInformation($"Connection string: '{connectionString}'");
    var match = ConnectionStringRegex().Match(connectionString);
    var dbPath = match.Groups["dbSource"].Value;
    logger.LogInformation($"Before Migration: Database path: '{Path.GetFullPath(dbPath)}', DB exists: {File.Exists(dbPath)}");
    
    logger.LogInformation("Migrating Database");
    await identityDb.Database.MigrateAsync();
    
    logger.LogInformation($"After Migration: Database path: '{Path.GetFullPath(dbPath)}', DB exists: {File.Exists(dbPath)}");

    logger.LogInformation("Adding default roles.");
    await CreateRoleAsync(options.Value.Roles.AdminRoleName);
    await CreateRoleAsync(options.Value.Roles.MemberRoleName);
    
    var adminUser = await userManager.FindByEmailAsync(options.Value.Defaults.AdminEmail);
    if (adminUser == null)
    {
      logger.LogInformation("Creating admin user");
      adminUser = new User
      {
        UserName = options.Value.Defaults.AdminUserName,
        Email = options.Value.Defaults.AdminEmail,
        EmailConfirmed = true
      };

      var result = await userManager.CreateAsync(adminUser, options.Value.Defaults.AdminPassword);
      adminUser = await userManager.FindByNameAsync(options.Value.Defaults.AdminUserName);
      logger.LogWarning($"Default admin account created: result='{result}' id='{adminUser?.Id}'");
    }
    else
    {
      logger.LogWarning($"User {options.Value.Defaults.AdminUserName} already exists!");
    }

    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, options.Value.Roles.AdminRoleName))
    {
      await userManager.AddToRoleAsync(adminUser, options.Value.Roles.AdminRoleName);
    }
  }

  private async Task CreateRoleAsync(string roleName)
  {
    var role = await roleManager.FindByNameAsync(roleName);
    if (role == null)
    {
      role = new Role(roleName);
      var result = await roleManager.CreateAsync(role);
      logger.LogWarning($"Default role {roleName} created: result='{result}' id='{role.Id}'");
    }
    else
    {
      logger.LogWarning($"Role '{roleName}' already exists.");
    }
  }
}