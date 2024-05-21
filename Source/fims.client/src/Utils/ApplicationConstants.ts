class ApiConstants {
  public PingAuth = "/api/pingauth";
  public LoginUseCookies = "/api/login?useCookies=true";
  public LoginUseSessionCookies = "/api/login?useSessionCookies=true";
}

class RouteConstants {
  public Login = "/login";
  public Home = "/home";
  public Logout = "/logout";
  public OAuth = "/oauth";
  public Register = "/register";
  public Root = "/";
}


export class ApplicationConstants {
  public static Routes : RouteConstants = new RouteConstants();
  public static Api : ApiConstants = new ApiConstants();
}