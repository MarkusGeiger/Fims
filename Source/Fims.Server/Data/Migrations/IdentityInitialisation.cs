using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fims.Server.Data.Migrations;

public class IdentityInitialisation(
  ILogger<IdentityInitialisation> logger,
  ApplicationDbContext db,
  UserManager<ApplicationUser> userManager//,
  //RoleManager<ApplicationRole> roleManager
  )
{
  public async Task Run()
  {
    logger.LogInformation("Migrating Database");
    await db.Database.MigrateAsync();
    var adminUser = new ApplicationUser
    {
      UserName = "admin@admin.com",
      Email = "admin@admin.com",
      EmailConfirmed = true
    };

    logger.LogInformation("Creating admin user");
    var result = await userManager.CreateAsync(adminUser, "Admin!23");
    adminUser = await userManager.FindByNameAsync(adminUser.UserName);
    logger.LogWarning($"Default admin account created: result='{result}' id='{adminUser?.Id}'");

    // logger.LogInformation("Adding default roles.");
    // await roleManager.CreateAsync(new ApplicationRole("admin"));
    // await roleManager.CreateAsync(new ApplicationRole("member"));
    // await userManager.AddToRoleAsync(adminUser!, "admin");
  }
}