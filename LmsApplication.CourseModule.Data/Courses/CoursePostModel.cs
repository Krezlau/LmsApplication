using LmsApplication.Core.Shared.Enums;

namespace LmsApplication.CourseModule.Data.Courses;

public class CoursePostModel
{
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public List<Guid> Categories { get; set; } = new();
    
    public CourseDuration Duration { get; set; }
}