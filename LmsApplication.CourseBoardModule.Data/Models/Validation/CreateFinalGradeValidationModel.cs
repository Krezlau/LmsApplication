using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Data.Models.Validation;

public class CreateFinalGradeValidationModel
{
    public required Guid CourseEditionId { get; set; } 
    
    public required UserExchangeModel? Teacher { get; set; }
    
    public required string StudentId { get; set; }
    
    public required decimal Value { get; set; }
}