using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Data.Enums;

namespace LmsApplication.Core.Data.Entities;

public class Course : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public CourseDuration Duration { get; set; }
    
    public virtual List<CourseCategory> Categories { get; set; } = new();

    public virtual List<CourseEdition> Editions { get; set; } = new();

    #region Audit

    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }

    #endregion
}