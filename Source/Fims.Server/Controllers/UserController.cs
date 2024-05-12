using System.Security.Claims;
using Fims.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fims.Server.Controllers;

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
  
  [HttpGet(Name = "GetUsers")]
  public async Task<IResult> Index()
  {
    var users = await _userManager.Users.ToListAsync();
    var userResultList =  users.Select(u => new
    {
      id = u.Id,
      email = u.Email,
      username = u.UserName,
      emailconfirmed = u.EmailConfirmed,
      roles = new List<string>()
    }).ToList();
    foreach (var userResult in userResultList)
    {
      var currentUser = await _userManager.FindByIdAsync(userResult.id);
      if (currentUser != null)
      {
        userResult.roles.AddRange(await _userManager.GetRolesAsync(currentUser));
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