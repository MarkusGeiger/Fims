var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloakContainer("keycloak").WithDataVolume();

var realm = keycloak.AddRealm("fims");

builder.AddProject<Projects.Fims_API>("api")
  .WithReference(keycloak)
  .WithReference(realm);


builder.AddProject<Projects.Fims_Server>("backend");
builder.Build().Run();