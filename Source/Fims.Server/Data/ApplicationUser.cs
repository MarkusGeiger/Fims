using Microsoft.AspNetCore.Identity;

namespace Fims.Server.Data;

public class ApplicationUser : IdentityUser
{
  public string AdditionalInformation { get; set; } = "";
}