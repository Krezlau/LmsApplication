using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models.Validation;

public class UpdateFinalGradeValidationModel
{
    public required FinalGrade? FinalGrade { get; set; }
    
    public required Guid CourseEditionId { get; set; } 
    
    public required UserExchangeModel? Teacher { get; set; }
    
    public required string StudentId { get; set; }
    
    public required decimal Value { get; set; }
}