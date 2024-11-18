namespace LmsApplication.CourseModule.Data.Courses;

public class CourseEditionSettingsModel
{
    public required Guid EditionId { get; set; }
    
    public required bool AllowAllToPost { get; set; }
}