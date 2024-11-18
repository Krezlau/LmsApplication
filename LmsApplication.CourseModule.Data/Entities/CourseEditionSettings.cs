using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Shared.Entities;

namespace LmsApplication.CourseModule.Data.Entities;

public class CourseEditionSettings : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public bool AllowAllToPost { get; set; }
    
    public Guid CourseEditionId { get; set; }

    [ForeignKey(nameof(CourseEditionId))]
    public virtual CourseEdition CourseEdition { get; set; } = null!;
    
    #region Audit
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }
    
    #endregion
}