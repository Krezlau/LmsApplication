using FluentValidation;
using LmsApplication.Core.Data.Enums;
using LmsApplication.Core.Data.Models.Courses;
using LmsApplication.Core.Services.Courses;
using LmsApplication.Core.Services.Graph;

namespace LmsApplication.Api.Shared.Validation;

public class CourseEditionAddUserModelValidator : AbstractValidator<CourseEditionAddUserModel>
{
    private readonly IGraphService _graphService;
    private readonly ICourseEditionService _courseEditionService;
    
    public CourseEditionAddUserModelValidator(IGraphService graphService, ICourseEditionService courseEditionService)
    {
        _graphService = graphService;
        _courseEditionService = courseEditionService;
        
        RuleFor(x => x.UserEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x)
            .CustomAsync(UserValidAsync);
    }

    private async Task UserValidAsync(CourseEditionAddUserModel model, ValidationContext<CourseEditionAddUserModel> context, CancellationToken ct)
    {
        var user = await _graphService.GetUserAsync(model.UserEmail);
        if (user is null)
        {
            context.AddFailure("User does not exist.");
            return;
        }

        Guid courseId;
        if (!context.RootContextData.TryGetValue(nameof(courseId), out var value) || !Guid.TryParse(value?.ToString(), out courseId))
        {
            context.AddFailure("Course does not exist.");
            return;
        }
        
        var course = await _courseEditionService.GetCourseEditionByIdAsync(courseId);
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