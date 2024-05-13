using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Fims.Server;

/// <summary>
/// Auth0 Backend-for-Frontend OAuth 
/// https://auth0.com/blog/backend-for-frontend-pattern-with-auth0-and-dotnet/#Backend-For-FrontEnd-in-ASP-NET-Core
/// </summary>
public static class ApplicationOAuthAuthenticationExtensions
{
  public static void AddOAuthAuthentication(this IServiceCollection services, IConfigurationSection section)
  {
    services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      }).AddCookie(o =>
      {
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.Cookie.SameSite = SameSiteMode.Strict;
        o.Cookie.HttpOnly = true;
      })
      .AddOpenIdConnect("Auth0", options => ConfigureOpenIdConnect(options, section));

    services.AddHttpClient();
  }

  private static void ConfigureOpenIdConnect(OpenIdConnectOptions options, IConfigurationSection section)
  {
    // Set the authority to your Auth0 domain
    // options.Authority = $"https://{Configuration["Auth0:Domain"]}";
    options.Authority = $"https://{section["Domain"]}";

    // Configure the Auth0 Client ID and Client Secret
    options.ClientId = section["ClientId"];
    options.ClientSecret = section["ClientSecret"];

    // Set response type to code
    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

    options.ResponseMode = OpenIdConnectResponseMode.FormPost;

    // Configure the scope
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("offline_access");
    options.Scope.Add("read:weather");
    
    // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
    // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
    options.CallbackPath = new PathString("/callback");

    // Configure the Claims Issuer to be Auth0
    options.ClaimsIssuer = "Auth0";

    // This saves the tokens in the session cookie
    options.SaveTokens = true;
    
    options.Events = new OpenIdConnectEvents
    {
        // handle the logout redirection
        OnRedirectToIdentityProviderForSignOut = (context) =>
        {
            var logoutUri = $"https://{section["Domain"]}/v2/logout?client_id={section["ClientId"]}";

            var postLogoutUri = context.Properties.RedirectUri;
            if (!string.IsNullOrEmpty(postLogoutUri))
            {
                if (postLogoutUri.StartsWith("/"))
                {
                    // transform to absolute
                    var request = context.Request;
                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                }
                logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
            }
            context.Response.Redirect(logoutUri);
            context.HandleResponse();

            return Task.CompletedTask;
        },
        OnRedirectToIdentityProvider = context => {
            context.ProtocolMessage.SetParameter("audience", section["ApiAudience"]);
            return Task.CompletedTask;
        }
    };
  }
}