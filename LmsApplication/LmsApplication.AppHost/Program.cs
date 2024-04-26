using LmsApplication.Resources.Yarp;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var authService = builder.AddProject<LmsApplication_Api_AuthService>("authService");

var apiGateway = builder.AddProject<LmsApplication_Api_Gateway>("apiGateway")
    .WithReference(authService);

builder.AddYarp("yarp")
    .WithEndpoint(8080, scheme: "http")
    .WithReference(authService)
    // .Route("u", target: authService, path: "/api/auth/weatherforecast");
    .LoadFromConfiguration("ReverseProxy");

builder.Build().Run();