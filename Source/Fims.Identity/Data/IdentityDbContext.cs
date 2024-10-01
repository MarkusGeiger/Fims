using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fims.Identity.Data;

public class IdentityDbContext : IdentityDbContext<User, Role, string>
{
  public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
  {
    
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    // Customization and default overrides here.
    // Rename ASP.NET Core Identity table names, etc.
  }
}