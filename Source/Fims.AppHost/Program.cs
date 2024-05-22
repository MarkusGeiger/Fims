var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.Fims_Server>("backend");
builder.Build().Run();