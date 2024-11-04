using FluentValidation;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Validation;

public class CourseEditionPostModelValidator : AbstractValidator<CourseEditionPostModel>
{
    private const string CourseDoesNotExistMessage = "Course does not exist.";
    private const string TitleUniqueMessage = "Title must be unique.";
    
    private readonly ICourseEditionRepository _courseEditionRepository;
    
    public CourseEditionPostModelValidator(ICourseEditionRepository courseEditionRepository)
    {
        _courseEditionRepository = courseEditionRepository;
        
        RuleFor(x => x.Title)
            .NotEmpty();
        
        RuleFor(x => x)
            .MustAsync(TitleUniqueAsync)
            .WithMessage(TitleUniqueMessage);
        
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

    private async Task<bool> TitleUniqueAsync(CourseEditionPostModel model, CancellationToken ct)
    {
        return await _courseEditionRepository.GetCourseEditionByCourseIdAndTitleAsync(model.Title, model.CourseId) is null;
    }

    private bool CourseExists(CourseEditionPostModel _, CourseEditionPostModel __, ValidationContext<CourseEditionPostModel> context)
    {
        return context.RootContextData.TryGetValue(nameof(Course), out var value) && value is Course _;
    }
}