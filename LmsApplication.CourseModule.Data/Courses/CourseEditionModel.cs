using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Extensions;

namespace LmsApplication.CourseModule.Data.Courses;

public class CourseEditionModel
{
    public required Guid Id { get; set; }

    public required CourseModel? Course { get; set; }
    
    public required string Title { get; set; }
    
    public required CourseDuration Duration { get; set; }
    
    public required DateTime? RegistrationStartDateUtc { get; set; }
    
    public required DateTime? RegistrationEndDateUtc { get; set; }
    
    public required CourseEditionStatus Status { get; set; }
    
    public required DateTime StartDateUtc { get; set; }
    
    public required DateTime EndDateUtc { get => StartDateUtc.Add(Duration.ToTimeSpan()); set { } }
    
    public required int StudentLimit { get; set; }
    
    public required List<string> TeacherIds { get; set; } = new();
    
    public required List<string> StudentIds { get; set; } = new();
}

public enum CourseEditionStatus
{
    Planned,
    RegistrationOpen,
    RegistrationClosed,
    InProgress,
    Finished
}