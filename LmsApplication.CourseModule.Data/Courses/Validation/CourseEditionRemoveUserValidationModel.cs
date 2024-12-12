using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Entities;

namespace LmsApplication.CourseModule.Data.Courses.Validation;

public class CourseEditionRemoveUserValidationModel
{
    public required UserExchangeModel? User { get; set; }
    
    public required CourseEdition? CourseEdition { get; set; }
}