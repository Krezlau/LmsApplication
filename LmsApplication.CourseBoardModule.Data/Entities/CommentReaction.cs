using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Shared.Entities;

namespace LmsApplication.CourseBoardModule.Data.Entities;

public class CommentReaction : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    
    public ReactionType ReactionType { get; set; }
    
    public Guid CommentId { get; set; }
    
    [ForeignKey(nameof(CommentId))]
    public virtual Comment Comment { get; set; } = null!;
    
    #region Audit
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }
    
    #endregion
}