using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fims.Server.Data;

public class IdentityInitialisation(
  ILogger<IdentityInitialisation> logger,
  ApplicationDbContext db,
  UserManager<ApplicationUser> userManager,
  RoleManager<ApplicationRole> roleManager,
  IOptions<ApplicationIdentityOptions> options)
{
  public async Task Run()
  {
    logger.LogInformation("Migrating Database");
    await db.Database.MigrateAsync();

    logger.LogInformation("Adding default roles.");
    await CreateRoleAsync(options.Value.Roles.AdminRoleName);
    await CreateRoleAsync(options.Value.Roles.MemberRoleName);
    
    var adminUser = await userManager.FindByNameAsync(options.Value.DefaultAdminUserName);
    if (adminUser == null)
    {
      logger.LogInformation("Creating admin user");
      adminUser = new ApplicationUser
      {
        UserName = options.Value.DefaultAdminUserName,
        Email = options.Value.DefaultAdminUserName,
        EmailConfirmed = true
      };

      var result = await userManager.CreateAsync(adminUser, options.Value.DefaultAdminPassword);
      adminUser = await userManager.FindByNameAsync(options.Value.DefaultAdminUserName);
      logger.LogWarning($"Default admin account created: result='{result}' id='{adminUser?.Id}'");
    }
    else
    {
      logger.LogWarning($"User {options.Value.DefaultAdminUserName} already exists!");
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
      role = new ApplicationRole(roleName);
      var result = await roleManager.CreateAsync(role);
      logger.LogWarning($"Default role {roleName} created: result='{result}' id='{role.Id}'");
    }
    else
    {
      logger.LogWarning($"Role '{roleName}' already exists.");
    }
  }
}