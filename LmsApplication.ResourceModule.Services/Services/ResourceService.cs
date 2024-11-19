using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.Core.Shared.Services;
using LmsApplication.ResourceModule.Data.Entities;
using LmsApplication.ResourceModule.Data.Mapping;
using LmsApplication.ResourceModule.Data.Models;
using LmsApplication.ResourceModule.Services.BlobClients;
using LmsApplication.ResourceModule.Services.Repositories;

namespace LmsApplication.ResourceModule.Services.Services;

public interface IResourceService
{
    Task<Stream> DownloadResourceAsync(Guid resourceId);
    
    Task<List<ResourceMetadataModel>> GetResourcesAsync(ResourceType resourceType, Guid parentId);
    
    Task<ResourceMetadataModel> UploadResourceAsync(ResourceUploadModel model);
    
    Task DeleteResourceAsync(Guid resourceId);
}

public class ResourceService : IResourceService
{
    private readonly IResourceMetadataRepository _resourceMetadataRepository;
    private readonly IBlobClient _blobClient;
    private readonly IUserProvider _userProvider;
    private readonly ICourseEditionProvider _courseEditionProvider;
    private readonly IValidationService<ResourceUploadModel> _resourceUploadModelValidationService;
    private readonly IUserContext _userContext;

    public ResourceService(
        IResourceMetadataRepository resourceMetadataRepository,
        IBlobClient blobClient,
        IUserProvider userProvider,
        ICourseEditionProvider courseEditionProvider,
        IValidationService<ResourceUploadModel> resourceUploadModelValidationService,
        IUserContext userContext)
    {
        _resourceMetadataRepository = resourceMetadataRepository;
        _blobClient = blobClient;
        _userProvider = userProvider;
        _courseEditionProvider = courseEditionProvider;
        _resourceUploadModelValidationService = resourceUploadModelValidationService;
        _userContext = userContext;
    }

    public async Task<List<ResourceMetadataModel>> GetResourcesAsync(ResourceType resourceType, Guid parentId)
    {
        await ValidateReadAccessToResourcesAsync(_userContext.GetUserId(), resourceType, parentId);
        
        var resourceMetadataList = await _resourceMetadataRepository.GetResourcesAsync(resourceType, parentId);
        
        var users = await _userProvider.GetUsersByIdsAsync(resourceMetadataList.Select(x => x.UserId).ToList());
        
        return resourceMetadataList.Select(x => x.ToModel(users[x.UserId])).ToList();
    }

    public async Task<Stream> DownloadResourceAsync(Guid resourceId)
    {
        var resourceMetadata = await _resourceMetadataRepository.GetResourceAsync(resourceId);
        if (resourceMetadata is null) 
            throw new KeyNotFoundException("Resource not found.");
        
        await ValidateReadAccessToResourcesAsync(_userContext.GetUserId(), resourceMetadata.Type, resourceMetadata.ParentId);
        
        return await _blobClient.DownloadBlobAsync(resourceMetadata);
    }

    public async Task<ResourceMetadataModel> UploadResourceAsync(ResourceUploadModel model)
    {
        await _resourceUploadModelValidationService.ValidateAndThrowAsync(model);
        
        var userId = _userContext.GetUserId();
        var user = await _userProvider.GetUserByIdAsync(userId);
        await ValidateWriteAccessToResourcesAsync(user, model.Type, model.ParentId);
        
        var resourceMetadata = new ResourceMetadata
        {
            Type = model.Type,
            FileDisplayName = model.FileDisplayName,
            FileExtension = Path.GetExtension(model.File.FileName),
            FileSize = model.File.Length,
            ParentId = model.ParentId,
            UserId = userId,
        };

        await _resourceMetadataRepository.CreateAsync(resourceMetadata);

        try
        {
            await _blobClient.UploadBlobAsync(resourceMetadata, model.File);
        }
        catch (Exception)
        {
            await _resourceMetadataRepository.DeleteAsync(resourceMetadata);
            throw;
        }

        return resourceMetadata.ToModel(user!);
    }

    public async Task DeleteResourceAsync(Guid resourceId)
    {
        var resourceMetadata = await _resourceMetadataRepository.GetResourceAsync(resourceId);
        if (resourceMetadata is null) 
            throw new KeyNotFoundException("Resource not found.");
        
        await ValidateWriteAccessToResourcesAsync(_userContext.GetUserId(), resourceMetadata.Type, resourceMetadata.ParentId);
        
        await _blobClient.DeleteBlobAsync(resourceMetadata);
        
        await _resourceMetadataRepository.DeleteAsync(resourceMetadata);
    }
    
    private async Task ValidateReadAccessToResourcesAsync(string userId, ResourceType resourceType, Guid parentId)
    {
        var isAdmin = _userContext.GetUserRole() is UserRole.Admin;
        if (isAdmin)
            return;

        switch (resourceType)
        {
            case ResourceType.Course:
                // permit all
                break;
            
            case ResourceType.Edition:
                var isParticipant = await _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(parentId, userId);
                if (!isParticipant)
                    throw new UnauthorizedAccessException("User is not registered to course edition.");
                break;
        }
    }
    
    private async Task ValidateWriteAccessToResourcesAsync(string userId, ResourceType resourceType, Guid parentId)
    {
        // we know that user is either an admin or a teacher 
        var isAdmin = _userContext.GetUserRole() is UserRole.Admin;
        if (isAdmin)
            return;

        await ValidateNonAdminWriteAccessAsync(userId, resourceType, parentId);
    }
    
    private async Task ValidateWriteAccessToResourcesAsync(UserExchangeModel? user, ResourceType resourceType, Guid parentId)
    {
        if (user is null) 
            throw new UnauthorizedAccessException("User not found.");
        
        // we know that user is either an admin or a teacher 
        if (user.Role is UserRole.Admin)
            return;

        await ValidateNonAdminWriteAccessAsync(user.Id, resourceType, parentId);
    }


    private async Task ValidateNonAdminWriteAccessAsync(string userId, ResourceType resourceType, Guid parentId)
    {
        switch (resourceType)
        {
            case ResourceType.Course:
                // permit only admins
                throw new UnauthorizedAccessException("User is not an admin.");
                break;
            
            case ResourceType.Edition:
                var isParticipant = await _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(parentId, userId);
                if (!isParticipant)
                    throw new UnauthorizedAccessException("User is not registered to course edition.");
                break;
        }
    }
}