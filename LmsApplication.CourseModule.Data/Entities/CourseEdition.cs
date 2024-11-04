using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LmsApplication.Core.Shared.Entities;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Extensions;

namespace LmsApplication.CourseModule.Data.Entities;

public class CourseEdition : IAuditable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public Guid CourseId { get; set; }

    [ForeignKey(nameof(CourseId))] 
    public virtual Course? Course { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public CourseDuration Duration { get; set; }
    
    public DateTime StartDateUtc { get; set; }
    
    public DateTime EndDateUtc { get => StartDateUtc.Add(Duration.ToTimeSpan()); set { } }
    
    public int StudentLimit { get; set; }
    
    public List<string> TeacherEmails => Participants.Where(p => p.ParticipantRole == UserRole.Teacher).Select(p => p.ParticipantEmail).ToList(); 
    
    public List<string> StudentEmails => Participants.Where(p => p.ParticipantRole == UserRole.Student).Select(p => p.ParticipantEmail).ToList();

    public virtual List<CourseEditionParticipant> Participants { get; set; } = new();
    
    #region Audit
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime? DeletedAtUtc { get; set; }

    #endregion
}