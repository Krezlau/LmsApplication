using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Shared.Entities;

namespace LmsApplication.CourseBoardModule.Data.Entities;

public class GradesTableRowDefinition : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public required Guid CourseEditionId { get; set; }
    
    public required string Title { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime? Date { get; set; }
    
    public required RowType RowType { get; set; }
    
    public bool IsSummed { get; set; }
    
    #region Audit
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }
    
    #endregion
}