using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fims.Server.Data.Migrations;

public class IdentityInitialisation(
  ILogger<IdentityInitialisation> logger,
  ApplicationDbContext db,
  UserManager<ApplicationUser> userManager,
  RoleManager<ApplicationRole> roleManager
  )
{
  public async Task Run()
  {
    logger.LogInformation("Migrating Database");
    await db.Database.MigrateAsync();
    
    // Configuration
    var adminUserName = "admin@admin.com";
    var adminPassword = "Admin!23";
    var adminRoleName = "admin";
    var memberRoleName = "member";

    logger.LogInformation("Adding default roles.");
    await CreateRoleAsync(adminRoleName);
    await CreateRoleAsync(memberRoleName);
    
    var adminUser = await userManager.FindByNameAsync(adminUserName);
    if (adminUser == null)
    {
      logger.LogInformation("Creating admin user");
      adminUser = new ApplicationUser
      {
        UserName = adminUserName,
        Email = adminUserName,
        EmailConfirmed = true
      };

      var result = await userManager.CreateAsync(adminUser, adminPassword);
      adminUser = await userManager.FindByNameAsync(adminUser.UserName);
      logger.LogWarning($"Default admin account created: result='{result}' id='{adminUser?.Id}'");
    }
    else
    {
      logger.LogWarning($"User {adminUser.UserName} already exists!");
    }

    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, adminRoleName))
    {
      await userManager.AddToRoleAsync(adminUser, adminRoleName);
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