using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Courses;

namespace LmsApplication.CourseModule.Services.Validation;

public class CourseEditionRegisterModelValidator : AbstractValidator<CourseEditionRegisterModel>
{
    public CourseEditionRegisterModelValidator()
    {
        RuleFor(x => x)
            .Custom(IsValid);
    }

    private void IsValid(CourseEditionRegisterModel model, ValidationContext<CourseEditionRegisterModel> context)
    {
        if (model.User is null)
        {
            context.AddFailure("User does not exist.");
            return;
        }

        if (model.CourseEdition is null)
        {
            context.AddFailure("Course edition does not exist.");
            return;
        }
        
        if (model.User.Role is not UserRole.Student)
        {
            context.AddFailure("Can't register as a teacher.");
            return;
        }
        
        if (model.CourseEdition.StudentEmails.Contains(model.User.Email))
        {
            context.AddFailure("You're already a student of this course.");
            return;
        }
        
        if (model.CourseEdition.Status is not CourseEditionStatus.RegistrationOpen)
        {
            context.AddFailure("Course edition registration is closed.");
            return;
        }
        
        if (model.CourseEdition.StudentEmails.Count >= model.CourseEdition.StudentLimit)
        {
            context.AddFailure("Course edition is full.");
        }
    }
}