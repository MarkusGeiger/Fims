using Microsoft.AspNetCore.Identity;

namespace Fims.Identity.Data;

public class User : IdentityUser
{
  public string AdditionalInformation { get; set; } = "";
}