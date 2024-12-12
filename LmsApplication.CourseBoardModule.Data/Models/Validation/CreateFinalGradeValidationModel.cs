using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Data.Models.Validation;

public class CreateFinalGradeValidationModel
{
    public required CourseEditionModel? CourseEdition { get; set; } 
    
    public required UserExchangeModel? Teacher { get; set; }
    
    public required UserExchangeModel? Student { get; set; }
    
    public required decimal Value { get; set; }
}