using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.Providers;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class UpdateFinalGradeValidator : AbstractValidator<UpdateFinalGradeValidationModel>
{
    private readonly ICourseEditionProvider _courseEditionProvider;
    private static readonly decimal[] ValidValues = [2.0m, 2.5m, 3.0m, 3.5m, 4.0m, 4.5m, 5.0m];

    public UpdateFinalGradeValidator(ICourseEditionProvider courseEditionProvider)
    {
        _courseEditionProvider = courseEditionProvider;
        
        RuleFor(x => x.FinalGrade)
            .NotNull()
            .WithMessage("Could not find final grade.");

        RuleFor(x => x.CourseEditionId)
            .MustAsync(CourseEditionExistsAsync)
            .WithMessage("Could not find course edition.");
        
        RuleFor(x => x.StudentId)
            .MustAsync(StudentIsEnrolledAsync)
            .WithMessage("Student is not enrolled in this course edition.");

        RuleFor(x => x.Teacher)
            .NotNull()
            .WithMessage("Could not find teacher.");

        RuleFor(x => x.Value)
            .Must(ValueValid)
            .WithMessage("Final grade must be one of the following: 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0.");
    }

    private Task<bool> StudentIsEnrolledAsync(UpdateFinalGradeValidationModel model, string userId, CancellationToken ct)
    {
        return _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(model.CourseEditionId, userId);
    }

    private async Task<bool> CourseEditionExistsAsync(Guid courseEditionId, CancellationToken ct)
    {
        return await _courseEditionProvider.CourseEditionExistsAsync(courseEditionId);
    }

    private static bool ValueValid(decimal arg)
    {
        return ValidValues.Contains(arg);
    }
}