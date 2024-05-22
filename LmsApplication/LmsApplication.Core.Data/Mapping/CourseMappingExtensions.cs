using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Models;
using LmsApplication.Core.Data.Models.Courses;

namespace LmsApplication.Core.Data.Mapping;

public static class CourseMappingExtensions
{
    public static CourseModel ToModel(this Course course)
    {
        return new CourseModel
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Categories = course.Categories.ToList(),
            Duration = course.Duration,
        };
    }
    
    public static CourseEditionModel ToModel(this CourseEdition courseEdition)
    {
        return new CourseEditionModel
        {
            Id = courseEdition.Id,
            CourseId = courseEdition.CourseId,
            Duration = courseEdition.Duration,
            StartDateUtc = courseEdition.StartDateUtc,
            StudentLimit = courseEdition.StudentLimit,
            TeacherIds = courseEdition.TeacherEmails.ToList(),
            StudentIds = courseEdition.StudentEmails.ToList(),
            EndDateUtc = courseEdition.EndDateUtc,
        };
    }
}