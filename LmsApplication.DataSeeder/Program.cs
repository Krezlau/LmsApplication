using System.CommandLine;
using LmsApplication.CourseBoardModule.Data;
using LmsApplication.CourseModule.Data;
using LmsApplication.DataSeeder.Services;
using LmsApplication.ResourceModule.Data;
using LmsApplication.UserModule.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(_ => configuration);
services.AddScoped<IDataSeederService, DataSeederService>();

services.AddUserModuleData(configuration);
services.AddCourseModuleData(configuration);
services.AddCourseBoardModuleData(configuration);
services.AddResourceModuleData(configuration);

var serviceProvider = services.BuildServiceProvider();

var rootCommand = new RootCommand("Data seeder");

var clearCommand = new Command("clear", "Clears the database");
rootCommand.AddCommand(clearCommand);

var seedCommand = new Command("seed", "Seeds the database");
rootCommand.AddCommand(seedCommand);

clearCommand.SetHandler(async () =>
{
    await serviceProvider.GetService<IDataSeederService>()!.ClearDatabaseAsync();
});

seedCommand.SetHandler(async () =>
{
    await serviceProvider.GetService<IDataSeederService>()!.SeedDataAsync();
});

rootCommand.InvokeAsync(args).Wait();