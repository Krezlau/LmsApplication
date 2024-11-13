using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var appconfig = builder.AddConnectionString("AppConfig");

builder.Configuration.AddAzureAppConfiguration(c =>
{
    c.Connect(builder.Configuration.GetConnectionString("AppConfig"))
        .Select(KeyFilter.Any);
});

// Services
var app = builder.AddProject<LmsApplication_Core_Api>("api")
    .WithReference(appconfig);

var password = builder.AddParameter("db-password", secret: true);

var sqlServer = builder.AddSqlServer("sql-db", password).WithDataVolume("sql-db");

var courseDb = sqlServer.AddDatabase("course-db");
var userDb = sqlServer.AddDatabase("user-db");
var courseBoardDb = sqlServer.AddDatabase("course-board-db");

app.WithReference(userDb);
app.WithReference(courseDb);
app.WithReference(courseBoardDb);

builder.Build().Run();
