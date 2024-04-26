using LmsApplication.Resources.Yarp;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var authService = builder.AddProject<LmsApplication_Api_AuthService>("authService");

var apiGateway = builder.AddProject<LmsApplication_Api_Gateway>("apiGateway")
    .WithReference(authService);

builder.AddYarp("yarp")
    .WithEndpoint(8080, scheme: "http")
    .WithReference(authService)
    .LoadFromConfiguration("ReverseProxy");

builder.Build().Run();