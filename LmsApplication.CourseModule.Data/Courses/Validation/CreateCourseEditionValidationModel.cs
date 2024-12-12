using LmsApplication.CourseModule.Data.Entities;

namespace LmsApplication.CourseModule.Data.Courses.Validation;

public class CreateCourseEditionValidationModel
{
    public required string Title { get; set; }
    
    public required Guid CourseId { get; set; }
    
    public required DateTime StartDateUtc { get; set; }
    
    public required int StudentLimit { get; set; }
    
    public required DateTime? RegistrationStartDateUtc { get; set; }
    
    public required DateTime? RegistrationEndDateUtc { get; set; }
    
    public required Course? Course { get; set; }
}