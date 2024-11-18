using LmsApplication.Core.Shared.Models;
using LmsApplication.ResourceModule.Data.Entities;
using LmsApplication.ResourceModule.Data.Models;

namespace LmsApplication.ResourceModule.Data.Mapping;

public static class ResourceMetadataMappingService
{
    public static ResourceMetadataModel ToModel(this ResourceMetadata entity, UserExchangeModel user)
    {
        return new ResourceMetadataModel
        {
            Id = entity.Id,
            FileDisplayName = entity.FileDisplayName,
            FileSize = entity.FileSize,
            FileExtension = entity.FileExtension,
            Type = entity.Type,
            ParentId = entity.ParentId,
            CreatedAtUtc = entity.CreatedAtUtc,
            User = user,
        };
    }
}