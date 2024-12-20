using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Entities;
using CourseEditionModel = LmsApplication.CourseModule.Data.Courses.CourseEditionModel;

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
    
    public static CourseEditionModel ToModel(this CourseEdition courseEdition, string currentUserId, FinalGradeModel? finalGrade = null)
    {
        return new CourseEditionModel
        {
            Id = courseEdition.Id,
            Title = courseEdition.Title,
            Duration = courseEdition.Duration,
            RegistrationStartDateUtc = courseEdition.RegistrationStartDateUtc,
            RegistrationEndDateUtc = courseEdition.RegistrationEndDateUtc,
            StartDateUtc = courseEdition.StartDateUtc,
            StudentLimit = courseEdition.StudentLimit,
            StudentCount = courseEdition.Participants.Count,
            EndDateUtc = courseEdition.EndDateUtc,
            Status = courseEdition.Status,
            Course = courseEdition.Course?.ToModel(),
            IsUserRegistered = courseEdition.Participants.Any(x => x.ParticipantId == currentUserId),
            Settings = courseEdition.Settings.ToPublicModel(),
            FinalGrade = finalGrade,
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