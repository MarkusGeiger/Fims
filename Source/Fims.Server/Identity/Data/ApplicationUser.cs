using Microsoft.AspNetCore.Identity;

namespace Fims.Server.Identity.Data;

public class ApplicationUser : IdentityUser
{
  public string AdditionalInformation { get; set; } = "";
}