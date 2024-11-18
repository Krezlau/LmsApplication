using LmsApplication.ResourceModule.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace LmsApplication.ResourceModule.Data.Models;

public class ResourceUploadModel
{
    public string FileDisplayName { get; set; }
    
    public IFormFile File { get; set; }
    
    public ResourceType Type { get; set; }
    
    public Guid ParentId { get; set; }
}