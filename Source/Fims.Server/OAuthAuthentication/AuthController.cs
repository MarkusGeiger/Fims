using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fims.Server.OAuthAuthentication;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  [HttpGet("login")]
  public ActionResult Login(string returnUrl = "/")
  {
    return new ChallengeResult("Auth0", new AuthenticationProperties() 
      { 
        RedirectUri = returnUrl
      }
    );
  }
  
  [Authorize]
  [HttpGet("logout")]
  public async Task<ActionResult> Logout()
  {
    await HttpContext.SignOutAsync();

    return new SignOutResult("Auth0", new AuthenticationProperties
    {
      RedirectUri = Url.Action("Index", "Home")
    });
  }
  
  [HttpGet("GetUser")]
  public IResult GetUser()
  {
    if (User.Identity is { IsAuthenticated: true })
    {
      var claims = ((ClaimsIdentity)this.User.Identity).Claims.Select(c =>
          new { type = c.Type, value = c.Value })
        .ToArray();

      return Results.Json(new { isAuthenticated = true, claims = claims });
    }

    return Results.Json(new { isAuthenticated = false });
  }
}