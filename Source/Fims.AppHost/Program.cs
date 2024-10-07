var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloakContainer("keycloak").WithDataVolume();

var realm = keycloak.AddRealm("fims");

var realm2 = keycloak.AddRealm("keycloak-auth-demo");

builder.AddProject<Projects.Fims_API>("api")
  .WithReference(keycloak)
  .WithReference(realm);

builder.AddProject<Projects.Fims_API2>("api2")
  .WithReference(keycloak)
  .WithReference(realm2);

builder.AddProject<Projects.Fims_Server>("backend");
builder.Build().Run();