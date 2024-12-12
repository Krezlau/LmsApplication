using FluentValidation;
using LmsApplication.CourseModule.Data.Courses.Validation;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Validation;

public class CourseEditionPostModelValidator : AbstractValidator<CreateCourseEditionValidationModel>
{
    private const string TitleUniqueMessage = "Title must be unique.";
    
    private readonly ICourseEditionRepository _courseEditionRepository;
    
    public CourseEditionPostModelValidator(ICourseEditionRepository courseEditionRepository)
    {
        _courseEditionRepository = courseEditionRepository;

        RuleFor(x => x.Course)
            .NotNull()
            .WithMessage("Course not found.");
        
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
            .Custom(RegistrationPeriodValid);
    }

    private void RegistrationPeriodValid(CreateCourseEditionValidationModel model, ValidationContext<CreateCourseEditionValidationModel> context)
    {
        if (model.RegistrationStartDateUtc is null && model.RegistrationEndDateUtc is null)
        {
            // course without registration period
            return;
        }
        if ((model.RegistrationStartDateUtc is null && model.RegistrationEndDateUtc is not null) ||
            (model.RegistrationStartDateUtc is not null && model.RegistrationEndDateUtc is null))
        {
            // only one of the dates is set
            context.AddFailure("RegistrationStartDateUtc",
                "For courses with registration period both registration start and end date must be set.");
            return;
        }
        
        // course with registration period
        if (model.RegistrationStartDateUtc >= model.StartDateUtc || model.RegistrationEndDateUtc >= model.StartDateUtc)
        {
            context.AddFailure("RegistrationStartDateUtc",
                "Registration start and end date must be before course start date.");
        }
        
        if (model.RegistrationStartDateUtc >= model.RegistrationEndDateUtc)
        {
            context.AddFailure("RegistrationStartDateUtc",
                "Registration start date must be before registration end date.");
        }
    }

    private async Task<bool> TitleUniqueAsync(CreateCourseEditionValidationModel model, CancellationToken ct)
    {
        return await _courseEditionRepository.GetCourseEditionByCourseIdAndTitleAsync(model.Title, model.CourseId) is null;
    }
}