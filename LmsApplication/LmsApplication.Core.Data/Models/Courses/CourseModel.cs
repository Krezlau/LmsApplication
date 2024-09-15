using LmsApplication.Core.Data.Enums;

namespace LmsApplication.Core.Data.Models.Courses;

public class CourseModel
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public List<CourseCategoryModel> Categories { get; set; } = new();
    
    public CourseDuration Duration { get; set; }
}