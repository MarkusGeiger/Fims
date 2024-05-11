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

  public UserController(ILogger<UserController> logger, UserManager<ApplicationUser> userManager)
  {
    _logger = logger;
    _userManager = userManager;
  }
  
  [HttpGet(Name = "GetUsers")]
  public async Task<IResult> Index()
  {
    var users = await _userManager.Users.Select(u => new
    {
      id = u.Id,
      email = u.Email,
      username = u.UserName,
      emailconfirmed = u.EmailConfirmed,
      //roles=await _userManager.GetRolesAsync(u)
    }).ToListAsync();
    return Results.Json(users);
  }

  [HttpDelete]
  public async Task<IResult> DeleteUser([FromQuery] string id)
  {
    var result = await _userManager.DeleteAsync(_userManager.Users.First(u => u.Id == id));

    return result.Succeeded ? Results.Ok() : Results.NotFound(result);
  }
}