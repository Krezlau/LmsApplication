using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var authService = builder.AddProject<LmsApplication_Api_AuthService>("authService");

var apiGateway = builder.AddProject<LmsApplication_Api_Gateway>("apiGateway")
    .WithReference(authService);

// builder.AddProject<Projects.LmsApplication_Web>("webfrontend")
    // .WithReference(apiGateway);

builder.Build().Run();