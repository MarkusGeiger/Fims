namespace Fims.Server;

public class ApplicationIdentityOptions
{
  public static readonly string Section = "Identity";
  public string DefaultAdminUserName = "admin@admin.com";
  public string DefaultAdminPassword = "Admin!23";
  public RoleOptions Roles = new();
}

public class RoleOptions
{
  public string AdminRoleName = "admin";
  public string MemberRoleName = "member";
}