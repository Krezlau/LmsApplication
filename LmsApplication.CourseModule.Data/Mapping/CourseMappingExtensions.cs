using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Entities;

namespace LmsApplication.CourseModule.Data.Mapping;

public static class CourseMappingExtensions
{
    public static CourseModel ToModel(this Course course)
    {
        return new CourseModel
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Categories = course.Categories.Select(x => x.ToModel()).ToList(),
            Duration = course.Duration,
        };
    }
    
    public static CourseEditionModel ToModel(this CourseEdition courseEdition)
    {
        return new CourseEditionModel
        {
            Id = courseEdition.Id,
            Duration = courseEdition.Duration,
            StartDateUtc = courseEdition.StartDateUtc,
            StudentLimit = courseEdition.StudentLimit,
            TeacherIds = courseEdition.TeacherEmails.ToList(),
            StudentIds = courseEdition.StudentEmails.ToList(),
            EndDateUtc = courseEdition.EndDateUtc,
            Course = courseEdition.Course?.ToModel(),
        };
    }
    
    public static CourseCategoryModel ToModel(this CourseCategory courseCategory)
    {
        return new CourseCategoryModel
        {
            Id = courseCategory.Id,
            Name = courseCategory.Name,
        };
    }
}