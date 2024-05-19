using LmsApplication.Core.Data.Enums;

namespace LmsApplication.Core.Data.Models;

public class CoursePostModel
{
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public List<string> Categories { get; set; } = new();
    
    public CourseDuration Duration { get; set; }
}