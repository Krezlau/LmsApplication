namespace LmsApplication.CourseModule.Data.Courses;

public class CourseEditionPostModel
{
    public Guid CourseId { get; set; }
    
    public DateTime StartDateUtc { get; set; }
    
    public int StudentLimit { get; set; }
}