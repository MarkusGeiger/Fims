using System.Security.Claims;
using Fims.Server.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fims.Server.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(
  ILogger<UserController> logger,
  UserManager<ApplicationUser> userManager,
  SignInManager<ApplicationUser> signinManager,
  RoleManager<ApplicationRole> roleManager,
  IOptions<ApplicationIdentityOptions> identityOptions)
  : ControllerBase
{
  [HttpPost("/api/logout")]
  [Authorize]
  public async Task<IResult> Logout()
  {
    await signinManager.SignOutAsync();
    return Results.Ok();
  }
  
  [HttpGet("/api/pingauth")]
  [Authorize]
  public IResult GetCurrentUserInformation()
  {
    // This is used by the frontend to acquire information about the logged in user,
    // that's stored inside the HTTP-only cookie in the browser and not accessible from JS
    var email = User.FindFirstValue(ClaimTypes.Email);
    return Results.Json(new { Email = email });
  }

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

  [HttpPut("/api/User/{userId}/role")]
  public async Task<IResult> SetRoleForUser([FromRoute] string userId, [FromBody] PutRoleForUserDto content)
  {
    var currentUserId = userManager.GetUserId(User);
    if (userId == currentUserId)
    {
      return Results.BadRequest("Cannot update role for current user.");
    }

    var role = await roleManager.Roles.SingleOrDefaultAsync(r => r.Id == content.RoleId);
    if (role == null) return Results.NotFound($"A role with the given id {content.RoleId} could not be found.");
    
    var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
    if (user == null) return Results.NotFound($"A user with the given id {userId} could not be found.");

    if (await userManager.IsInRoleAsync(user, role.Name!))
    {
      return Results.BadRequest($"User is already in given role {role.Id}");
    }

    var identityResult = await userManager.AddToRoleAsync(user, role.Name!);
    return identityResult.Succeeded ? Results.Ok(identityResult) : Results.BadRequest(identityResult);
  }
  
  [HttpPut("/api/User/{userId}")]
  public async Task<IResult> UpdateUser([FromRoute] string userId, [FromBody] PutUserDto content)
  {
    var currentUserId = userManager.GetUserId(User);
    if (userId == currentUserId)
    {
      return Results.BadRequest("Cannot update current user.");
    }
    
    var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
    if (user == null) return Results.NotFound($"A user with the given id {userId} could not be found.");

    if (user.UserName != content.UserName) user.UserName = content.UserName;
    if (user.Email != content.Email) user.Email = content.Email;

    var currentRoles = await userManager.GetRolesAsync(user);
    var rolesToRemove = currentRoles.Except(content.Roles).ToList();
    var rolesToAdd = content.Roles.Except(currentRoles).ToList();
    if (rolesToRemove.Any())
    {
      var removeFromRolesIdentityResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
      logger.LogDebug($"Remove roles [{string.Join(", ", rolesToRemove)}] from user {user.Id}. Result: {removeFromRolesIdentityResult}");
    }

    if (rolesToAdd.Any())
    {
      var addToRolesIdentityResult = await userManager.AddToRolesAsync(user, rolesToAdd);
      logger.LogDebug($"Add roles [{string.Join(", ", rolesToRemove)}] to user {user.Id}. Result: {addToRolesIdentityResult}");
    }
    
    var identityResult = await userManager.UpdateAsync(user);
    return identityResult.Succeeded ? Results.Ok(identityResult) : Results.BadRequest(identityResult);
  }

  [HttpGet(Name = "GetUsers")]
  public async Task<IResult> Index()
  {
    // Get all users from UserManager
    var users = await userManager.Users.ToListAsync();
    // Transform the application internal user objects into response objects
    var userResultList = users.Select(u => new GetUserResponseDto(u.Id, u.Email, u.UserName, u.EmailConfirmed, [])).ToList();
    // Add additional role information to all response objects
    foreach (var userResult in userResultList)
    {
      var currentUser = await userManager.FindByIdAsync(userResult.Id);
      if (currentUser != null)
      {
        userResult.Roles.AddRange(await userManager.GetRolesAsync(currentUser));
      }
    }
    return Results.Json(userResultList);
  }

  [HttpDelete]
  public async Task<IResult> DeleteUser([FromQuery] string id)
  {
    var userId = userManager.GetUserId(User);
    if (userId == id)
    {
      return Results.BadRequest("Cannot delete current user.");
    }
    var result = await userManager.DeleteAsync(userManager.Users.First(u => u.Id == id));

    return result.Succeeded ? Results.Ok() : Results.NotFound(result);
  }
  
  [HttpGet("/api/[controller]/Roles")]
  public async Task<IResult> GetUserRoles()
  {
    var roles = await roleManager.Roles.ToListAsync();
    if (!roles.Any())
    {
      return Results.NotFound();
    }

    //roles = new List<ApplicationRole> { new ApplicationRole("none") { Id = "" } }.Concat(roles).ToList();
    return Results.Ok(roles.Select(r => new GetRolesResponseDto(r.Id, r.Name)));
  }
}