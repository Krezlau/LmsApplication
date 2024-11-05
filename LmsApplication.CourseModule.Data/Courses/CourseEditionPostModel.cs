namespace LmsApplication.CourseModule.Data.Courses;

public class CourseEditionPostModel
{
    public string Title { get; set; } = string.Empty;
    
    public Guid CourseId { get; set; }
    
    public DateTime StartDateUtc { get; set; }
    
    public int StudentLimit { get; set; }
    
    public DateTime? RegistrationStartDateUtc { get; set; }
    
    public DateTime? RegistrationEndDateUtc { get; set; }
}