using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Entities;

namespace LmsApplication.CourseModule.Data.Courses;

public class CourseEditionRegisterModel
{
    public UserExchangeModel? User { get; set; }
    
    public CourseEdition? CourseEdition { get; set; }
}