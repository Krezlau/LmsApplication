using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Shared.Entities;

namespace LmsApplication.ResourceModule.Data.Entities;

public class ResourceMetadata : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public required string FileDisplayName { get; set; }
    
    public required long FileSize { get; set; }
    
    public required string FileExtension { get; set; }
    
    public required ResourceType Type { get; set; }
    
    public required Guid ParentId { get; set; }
    
    public required string UserId { get; set; }
    
    #region Audit
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }
    
    #endregion
}