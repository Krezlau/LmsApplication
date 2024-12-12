using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Courses.Validation;

namespace LmsApplication.CourseModule.Services.Validation;

public class CourseEditionAddUserModelValidator : AbstractValidator<CourseEditionAddUserValidationModel>
{
    public CourseEditionAddUserModelValidator()
    {
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("Could not find user.");
        
        RuleFor(x => x)
            .Custom(UserValid);
    }

    private static void UserValid(CourseEditionAddUserValidationModel model, ValidationContext<CourseEditionAddUserValidationModel> context)
    {
        var courseEdition = model.CourseEdition;
        if (courseEdition is null)
        {
            context.AddFailure("Course does not exist.");
            return;
        }
        
        if (courseEdition.Status is CourseEditionStatus.Finished)
        {
            context.AddFailure("Cannot modify users in finished course.");
            return;
        }
        
        switch (model.User?.Role)
        {
            case UserRole.Teacher when courseEdition.TeacherEmails.Contains(model.User!.Id):
                context.AddFailure("User is already a teacher of this course.");
                break;
            case UserRole.Student when courseEdition.StudentEmails.Contains(model.User!.Id):
                context.AddFailure("User is already a student of this course.");
                break;
        }
    }
}