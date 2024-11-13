using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Shared.Entities;

namespace LmsApplication.CourseBoardModule.Data.Entities;

public class Comment : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string Content { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    
    public Guid PostId { get; set; }
    
    [ForeignKey(nameof(PostId))]
    public virtual Post Post { get; set; } = null!;
    
    public virtual List<CommentReaction> Reactions { get; set; } = new();
    
    #region Audit
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }
    
    #endregion
}