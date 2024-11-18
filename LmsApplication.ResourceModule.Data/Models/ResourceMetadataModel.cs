using LmsApplication.Core.Shared.Models;
using LmsApplication.ResourceModule.Data.Entities;

namespace LmsApplication.ResourceModule.Data.Models;

public class ResourceMetadataModel
{
    public required Guid Id { get; set; }
    
    public required string FileDisplayName { get; set; }
    
    public required decimal FileSize { get; set; }
    
    public required string FileExtension { get; set; }
    
    public required ResourceType Type { get; set; }
    
    public required Guid ParentId { get; set; }
    
    public required DateTime CreatedAtUtc { get; set; }
    
    public required UserExchangeModel User { get; set; }
}