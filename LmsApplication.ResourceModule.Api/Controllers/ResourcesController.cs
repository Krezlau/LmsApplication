using System.Security.Claims;
using LmsApplication.Core.Shared.Config;
using LmsApplication.Core.Shared.Models;
using LmsApplication.ResourceModule.Data.Entities;
using LmsApplication.ResourceModule.Data.Models;
using LmsApplication.ResourceModule.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.ResourceModule.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpPost("upload")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> UploadResource([FromForm] ResourceUploadModel model)
    {
        return Ok(ApiResponseHelper.Success(await _resourceService.UploadResourceAsync(GetUserId(), model)));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> DownloadResource(Guid id)
    {
        return Ok(await _resourceService.DownloadResourceAsync(id, GetUserId()));
    }
    
    [HttpGet("metadatas/{resourceType}s/{parentId:guid}")]
    public async Task<IActionResult> GetResourceMetadatas(string resourceType, Guid parentId)
    {
        return Ok(ApiResponseHelper.Success(await _resourceService.GetResourcesAsync(GetUserId(), ParseResourceType(resourceType), parentId)));
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> DeleteResource(Guid id)
    {
        await _resourceService.DeleteResourceAsync(id, GetUserId());
        return Ok(ApiResponseHelper.Success());
    }
    
    private ResourceType ParseResourceType(string resourceType)
    {
        if (!Enum.TryParse<ResourceType>(resourceType, true, out var parsedResourceType))
            throw new ArgumentException("Incorrect resource type.");
        
        return parsedResourceType;
    }

    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) 
            throw new ArgumentException("Incorrect user id.");
        
        return userId;
    }
}