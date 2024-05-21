using System.Security.Claims;
using Fims.Server.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fims.Server.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
  private readonly ILogger<UserController> _logger;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly SignInManager<ApplicationUser> _signinManager;

  public UserController(ILogger<UserController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signinManager)
  {
    _logger = logger;
    _userManager = userManager;
    _signinManager = signinManager;
  }
  
  
  [HttpPost("/api/logout")]
  [Authorize]
  public async Task<IResult> Logout()
  {
    await _signinManager.SignOutAsync();
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

  [HttpGet(Name = "GetUsers")]
  public async Task<IResult> Index()
  {
    // Get all users from UserManager
    var users = await _userManager.Users.ToListAsync();
    // Transform the application internal user objects into response objects
    var userResultList = users.Select(u => new GetUserResponseDto(u.Id, u.Email, u.UserName, u.EmailConfirmed, [])).ToList();
    // Add additional role information to all response objects
    foreach (var userResult in userResultList)
    {
      var currentUser = await _userManager.FindByIdAsync(userResult.Id);
      if (currentUser != null)
      {
        userResult.Roles.AddRange(await _userManager.GetRolesAsync(currentUser));
      }
    }
    return Results.Json(userResultList);
  }

  [HttpDelete]
  public async Task<IResult> DeleteUser([FromQuery] string id)
  {
    var userId = _userManager.GetUserId(User);
    if (userId == id)
    {
      return Results.BadRequest("Cannot delete current user.");
    }
    var result = await _userManager.DeleteAsync(_userManager.Users.First(u => u.Id == id));

    return result.Succeeded ? Results.Ok() : Results.NotFound(result);
  }
}