using Microsoft.AspNetCore.Identity;

namespace Fims.Identity.Data;

public class Role : IdentityRole
{
  public Role()
  {}
  public Role(string roleName) : base(roleName){}
}