using LmsApplication.Core.ApplicationServices;
using LmsApplication.Core.Config;
using LmsApplication.Core.Config.Swagger;
using LmsApplication.Core.Data;
using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Services;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(c =>
{
    c.Connect(builder.Configuration.GetConnectionString("AppConfig"))
        .Select(KeyFilter.Any);
});

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new() { Title = "LmsApplication.Api.AuthService", Version = "v1" });
    opt.OperationFilter<TenantIdHeaderSwaggerAttribute>();
});

builder.Services.AddCoreApplicationServices();
builder.Services.AddCoreServices();
builder.Services.ConfigureModels(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.AddAuthDatabase();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapIdentityApi<User>();

app.Run();
