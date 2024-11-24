using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Shared.Entities;

namespace LmsApplication.CourseBoardModule.Data.Entities;

public abstract class GradesTableRowValue : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string UserId { get; set; }
    
    public Guid RowDefinitionId { get; set; }
    
    [ForeignKey(nameof(RowDefinitionId))]
    public virtual GradesTableRowDefinition RowDefinition { get; set; }
    
    public RowType RowType { get; set; }
    
    public string TeacherComment { get; set; }
    
    public string TeacherId { get; set; }
    
    #region Audit 
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }
    
    #endregion
}