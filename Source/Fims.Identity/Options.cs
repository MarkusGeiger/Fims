namespace Fims.Identity;

public class Options
{
  public static readonly string Section = "Identity";
  public string DefaultAdminUserName = "Administrator";
  public string DefaultAdminEmail = "admin@admin.com";
  public string DefaultAdminPassword = "Admin!23";
  public RoleOptions Roles = new();
}

public class RoleOptions
{
  public string AdminRoleName = "admin";
  public string MemberRoleName = "member";
}