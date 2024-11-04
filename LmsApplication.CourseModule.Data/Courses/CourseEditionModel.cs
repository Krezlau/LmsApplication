using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Extensions;

namespace LmsApplication.CourseModule.Data.Courses;

public class CourseEditionModel
{
    public Guid Id { get; set; }

    public CourseModel? Course { get; set; } = new();
    
    public string Title { get; set; } = string.Empty;
    
    public CourseDuration Duration { get; set; }
    
    public DateTime StartDateUtc { get; set; }
    
    public DateTime EndDateUtc { get => StartDateUtc.Add(Duration.ToTimeSpan()); set { } }
    
    public int StudentLimit { get; set; }
    
    public List<string> TeacherIds { get; set; } = new();
    
    public List<string> StudentIds { get; set; } = new();
}