using LmsApplication.Core.Config;
using LmsApplication.Core.Data;
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
builder.Services.AddSwaggerGen();
builder.Services.AddCoreServices();
builder.Services.ConfigureModels(builder.Configuration);
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

app.Run();
