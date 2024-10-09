using FluentValidation;
using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Models.Courses;

namespace LmsApplication.Api.Shared.Validation;

public class CourseEditionPostModelValidator : AbstractValidator<CourseEditionPostModel>
{
    private const string CourseDoesNotExistMessage = "Course does not exist.";
    
    public CourseEditionPostModelValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty();
        
        RuleFor(x => x.StartDateUtc)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow);
        
        RuleFor(x => x.StudentLimit)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x)
            .Must(CourseExists)
            .WithMessage(CourseDoesNotExistMessage);
    }

    private bool CourseExists(CourseEditionPostModel _, CourseEditionPostModel __, ValidationContext<CourseEditionPostModel> context)
    {
        return context.RootContextData.TryGetValue(nameof(Course), out var value) && value is Course _;
    }
}