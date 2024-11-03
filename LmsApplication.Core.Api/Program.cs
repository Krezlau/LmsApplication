using LmsApplication.Core.Api.Middleware;
using LmsApplication.Core.Shared.Config;
using LmsApplication.CourseModule.Api;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.UserModule.Api;
using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddAzureAppConfiguration(c =>
{
    c.Connect(builder.Configuration.GetConnectionString("AppConfig"))
        .Select(KeyFilter.Any);
});

builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpLogging(o => { });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCourseModuleApi(builder.Configuration);
builder.Services.AddUserModuleApi(builder.Configuration);


builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new() { Title = "LmsApplication.Api", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});


builder.Services.AddLogging();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandler>();

app.MapControllers();
app.MapCustomIdentityApi<User>();

app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var courseContext = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    courseContext.Database.Migrate();
    var userContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    userContext.Database.Migrate();
    
    // seed roles like this for now 
    // should be changed later
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var adminRole = new IdentityRole("Admin");
    var teacherRole = new IdentityRole("Teacher");
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(adminRole);
    if (!await roleManager.RoleExistsAsync("Teacher"))
        await roleManager.CreateAsync(teacherRole);

    var admin = userContext.Users.FirstOrDefault(u => u.Email == "krez@gmail.com");
    if (admin is not null)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await userManager.AddToRolesAsync(admin, new[] { "Admin", "Teacher" });
    }
    Console.WriteLine("Migrations applied successfully.");
}

app.Run();

