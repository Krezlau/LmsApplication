using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Validation;

public class CourseEditionRemoveUserModelValidator : AbstractValidator<CourseEditionRemoveUserModel>
{
    private readonly ICourseEditionRepository _courseEditionRepository;
    
    public CourseEditionRemoveUserModelValidator(ICourseEditionRepository courseEditionRepository)
    {
        _courseEditionRepository = courseEditionRepository;

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x)
            .CustomAsync(UserValidAsync);
    }

    private async Task UserValidAsync(CourseEditionRemoveUserModel model, ValidationContext<CourseEditionRemoveUserModel> context, CancellationToken ct)
    {
        Guid courseId;
        if (!context.RootContextData.TryGetValue(nameof(courseId), out var value) || !Guid.TryParse(value?.ToString(), out courseId))
        {
            context.AddFailure("Course does not exist.");
            return;
        }
        
        var course = await _courseEditionRepository.GetCourseEditionByIdAsync(courseId);
        if (course is null)
        {
            context.AddFailure("Course does not exist.");
            return;
        }

        if (!context.RootContextData.TryGetValue("user", out var userValue) || userValue is not UserExchangeModel user)
        {
            context.AddFailure("Could not find user.");
            return;
        }
        
        switch (user.Role)
        {
            case UserRole.Teacher when !course.TeacherEmails.Contains(model.UserId):
                context.AddFailure("User isn't a teacher of this course.");
                break;
            case UserRole.Student when !course.StudentEmails.Contains(model.UserId):
                context.AddFailure("User is not a member of this course.");
                break;
        }
    }
    
}