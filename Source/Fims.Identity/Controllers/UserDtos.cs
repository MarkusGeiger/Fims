namespace Fims.Identity.Controllers;

public record GetUserResponseDto(string Id, string? Email, string? Username, bool EmailConfirmed, List<string> Roles)
{
  public override string ToString()
  {
    return $"{{ id = {Id}, email = {Email}, username = {Username}, emailConfirmed = {EmailConfirmed}, roles = {Roles} }}";
  }
}

public record GetRolesResponseDto(string Id, string? Name)
{
  public override string ToString()
  {
    return $"{{ id = {Id}, name = {Name} }}";
  }
}

public record PutRoleForUserDto(string RoleId);

public record PutUserDto(string UserName, string Email, string[] Roles);
