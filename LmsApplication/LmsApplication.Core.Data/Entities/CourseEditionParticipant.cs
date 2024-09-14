using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Data.Enums;

namespace LmsApplication.Core.Data.Entities;

public class CourseEditionParticipant : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public Guid CourseEditionId { get; set; }
    
    public string ParticipantEmail { get; set; } = string.Empty;

    public UserRole ParticipantRole { get; set; } = UserRole.Student;
    
    #region Audit
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    #endregion
}