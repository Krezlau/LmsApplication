using LmsApplication.Core.Shared.Enums;

namespace LmsApplication.Core.Shared.Models;

public class CourseEditionModel
{
    public required Guid Id { get; set; }
    
    public required string Name { get; set; }
    
    public required CourseEditionStatus Status { get; set; }
}