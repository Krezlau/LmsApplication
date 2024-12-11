using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models.Validation;

public class UpdateRowValueValidationModel
{
    public required GradesTableRowDefinition? RowDefinition { get; set; }
    
    public required UserExchangeModel? Teacher { get; set; }
    
    public required UserExchangeModel? Student { get; set; }
    
    public required CourseEditionModel? CourseEdition { get; set; }
    
    public required string Value { get; set; }
    
    public required string? TeacherComment { get; set; }
}