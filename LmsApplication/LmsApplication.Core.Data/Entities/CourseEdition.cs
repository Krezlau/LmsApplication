using System.ComponentModel.DataAnnotations;
using LmsApplication.Core.Data.Enums;
using LmsApplication.Core.Data.Extensions;

namespace LmsApplication.Core.Data.Entities;

public class CourseEdition : IAuditable
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid CourseId { get; set; }
    
    public CourseDuration Duration { get; set; }
    
    public DateTime StartDateUtc { get; set; }
    
    public DateTime EndDateUtc { get => StartDateUtc.Add(Duration.ToTimeSpan()); set { } }
    
    public int StudentLimit { get; set; }
    
    public List<string> TeacherEmails { get; set; } = new();
    
    public List<string> StudentEmails { get; set; } = new();

    public Audit Audit { get; set; } = new();
}