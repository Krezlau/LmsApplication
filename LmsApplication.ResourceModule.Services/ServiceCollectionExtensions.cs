using Azure.Storage.Blobs;
using FluentValidation;
using LmsApplication.Core.Shared.Services;
using LmsApplication.ResourceModule.Data.Models;
using LmsApplication.ResourceModule.Services.BlobClients;
using LmsApplication.ResourceModule.Services.Repositories;
using LmsApplication.ResourceModule.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using BlobClient = LmsApplication.ResourceModule.Services.BlobClients.BlobClient;

namespace LmsApplication.ResourceModule.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResourceModuleServices(this IServiceCollection services)
    {
        services.AddScoped<IResourceService, ResourceService>();

        services.AddScoped<IResourceMetadataRepository, ResourceMetadataRepository>();

        services.AddScoped<IBlobClient, BlobClient>();
        services.AddScoped<BlobContainerClient>(_ => new BlobContainerClient("", "resources"));

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<IValidationService<ResourceUploadModel>, ValidationService<ResourceUploadModel>>();
            
        return services;
    }
}