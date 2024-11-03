using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Validation;

public class CourseEditionAddUserModelValidator : AbstractValidator<CourseEditionAddUserModel>
{
    private readonly ICourseEditionRepository _courseEditionRepository;
    
    public CourseEditionAddUserModelValidator(ICourseEditionRepository courseEditionRepository)
    {
        _courseEditionRepository = courseEditionRepository;
        
        RuleFor(x => x.UserEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x)
            .CustomAsync(UserValidAsync);
    }

    private async Task UserValidAsync(CourseEditionAddUserModel model, ValidationContext<CourseEditionAddUserModel> context, CancellationToken ct)
    {
        // var user = await _graphService.GetUserAsync(model.UserEmail);
        // if (user is null)
        // {
        //     context.AddFailure("User does not exist.");
        //     return;
        // }

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

        if (!context.RootContextData.TryGetValue(nameof(UserRole), out var role) || role is not UserRole userRole)
        {
            context.AddFailure("Incorrect user role.");
            return;
        }
        
        switch (userRole)
        {
            case UserRole.Teacher when course.TeacherEmails.Contains(model.UserEmail):
                context.AddFailure("User is already a teacher of this course.");
                break;
            case UserRole.Student when course.StudentEmails.Contains(model.UserEmail):
                context.AddFailure("User is already a student of this course.");
                break;
        }
    }
}