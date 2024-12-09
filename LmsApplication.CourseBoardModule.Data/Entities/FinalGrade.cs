using LmsApplication.Core.Shared.Entities;

namespace LmsApplication.CourseBoardModule.Data.Entities;

public class FinalGrade : IAuditable
{
    public Guid Id { get; set; }
    
    public Guid CourseEditionId { get; set; }
    
    public string UserId { get; set; }
    
    public decimal Value { get; set; }
    
    public string TeacherId { get; set; }
    
    #region Audit
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }
    
    #endregion
}