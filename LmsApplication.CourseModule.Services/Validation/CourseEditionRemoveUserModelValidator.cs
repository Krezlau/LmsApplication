using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Courses.Validation;

namespace LmsApplication.CourseModule.Services.Validation;

public class CourseEditionRemoveUserModelValidator : AbstractValidator<CourseEditionRemoveUserValidationModel>
{
    public CourseEditionRemoveUserModelValidator()
    {
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User not found.");

        RuleFor(x => x.CourseEdition)
            .NotNull()
            .WithMessage("Course edition not found.");

        RuleFor(x => x)
            .Custom(UserValid);
    }

    private static void UserValid(CourseEditionRemoveUserValidationModel model, ValidationContext<CourseEditionRemoveUserValidationModel> context)
    {
        if (model.CourseEdition is null || model.User is null)
            return;

        if (model.CourseEdition.Status is CourseEditionStatus.Finished)
        {
            context.AddFailure("Cannot modify users in finished course edition.");
            return;
        }
        
        switch (model.User.Role)
        {
            case UserRole.Teacher when !model.CourseEdition.TeacherEmails.Contains(model.User.Id):
                context.AddFailure("User isn't a teacher of this course.");
                break;
            case UserRole.Student when !model.CourseEdition.StudentEmails.Contains(model.User.Id):
                context.AddFailure("User is not a member of this course.");
                break;
        }
    }
    
}