
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Shared.Entities;

namespace LmsApplication.CourseBoardModule.Data.Entities;

public class Post : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    
    public Guid EditionId { get; set; }
    
    public virtual List<Comment> Comments { get; set; } = new();

    public virtual List<PostReaction> Reactions { get; set; } = new();

    #region Audit

    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }

    #endregion
}