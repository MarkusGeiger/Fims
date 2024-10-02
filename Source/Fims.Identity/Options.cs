namespace Fims.Identity;

public class Options
{
  public static readonly string Section = "Identity";
  public RoleOptions Roles = new();
  public DefaultOptions Defaults = new DefaultOptions();
}

public class DefaultOptions
{
  public string AdminUserName = "Administrator";
  public string AdminEmail = "admin@admin.com";
  public string AdminPassword = "Admin!23";
}

public class RoleOptions
{
  public string AdminRoleName = "admin";
  public string MemberRoleName = "member";
}