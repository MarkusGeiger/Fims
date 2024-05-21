using Microsoft.AspNetCore.Identity;

namespace Fims.Server.Identity.Data;

public class ApplicationRole : IdentityRole
{
  public ApplicationRole() : base(){}
  public ApplicationRole(string roleName) : base(roleName){}
}