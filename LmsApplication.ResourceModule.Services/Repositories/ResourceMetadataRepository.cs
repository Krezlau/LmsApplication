using LmsApplication.ResourceModule.Data.Database;
using LmsApplication.ResourceModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.ResourceModule.Services.Repositories;

public interface IResourceMetadataRepository
{
    Task<ResourceMetadata?> GetResourceAsync(Guid resourceId);
    
    Task<List<ResourceMetadata>> GetResourcesAsync(ResourceType resourceType, Guid parentId);
    
    Task CreateAsync(ResourceMetadata resourceMetadata);
    
    Task DeleteAsync(ResourceMetadata resourceMetadata);
}

public class ResourceMetadataRepository : IResourceMetadataRepository
{
    private readonly ResourceDbContext _dbContext;

    public ResourceMetadataRepository(ResourceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResourceMetadata?> GetResourceAsync(Guid resourceId)
    {
        return await _dbContext.ResourceMetadata.FirstOrDefaultAsync(x => x.Id == resourceId);
    }

    public async Task<List<ResourceMetadata>> GetResourcesAsync(ResourceType resourceType, Guid parentId)
    {
        return await _dbContext.ResourceMetadata
            .Where(x => x.Type == resourceType && x.ParentId == parentId)
            .ToListAsync();
    }

    public async Task CreateAsync(ResourceMetadata resourceMetadata)
    {
        await _dbContext.ResourceMetadata.AddAsync(resourceMetadata);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ResourceMetadata resourceMetadata)
    {
        _dbContext.ResourceMetadata.Remove(resourceMetadata);
        await _dbContext.SaveChangesAsync();
    }
}