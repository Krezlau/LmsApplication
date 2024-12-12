using FluentValidation;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class CreateFinalGradeValidator : AbstractValidator<CreateFinalGradeValidationModel>
{
    private readonly ICourseEditionProvider _courseEditionProvider;
    private readonly IFinalGradeRepository _finalGradeRepository;
    
    private static readonly decimal[] ValidValues = [2.0m, 2.5m, 3.0m, 3.5m, 4.0m, 4.5m, 5.0m];
    
    public CreateFinalGradeValidator(ICourseEditionProvider courseEditionProvider, IFinalGradeRepository finalGradeRepository)
    {
        _courseEditionProvider = courseEditionProvider;
        _finalGradeRepository = finalGradeRepository;

        RuleFor(x => x)
            .MustAsync(DoesNotYetHaveFinalGradeAsync)
            .WithMessage("Student already has a final grade.");

        RuleFor(x => x.CourseEdition)
            .NotNull()
            .WithMessage("Could not find course edition.");
        
        RuleFor(x => x.Student)
            .NotNull()
            .WithMessage("Could not find student.");
        
        RuleFor(x => x.Student)
            .MustAsync(StudentIsEnrolledAsync)
            .WithMessage("Student is not enrolled in this course edition.");

        RuleFor(x => x.Teacher)
            .NotNull()
            .WithMessage("Could not find teacher.");

        RuleFor(x => x.Value)
            .Must(ValueValid)
            .WithMessage("Final grade must be one of the following: 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0.");
    }

    private async Task<bool> DoesNotYetHaveFinalGradeAsync(CreateFinalGradeValidationModel model, CancellationToken ct)
    {
        if (model.CourseEdition is null || model.Student is null) 
            return false;
        
        return !await _finalGradeRepository.GradeExistsAsync(model.CourseEdition.Id, model.Student.Id);
    }

    private async Task<bool> StudentIsEnrolledAsync(CreateFinalGradeValidationModel model, UserExchangeModel? student, CancellationToken ct)
    {
        if (model.CourseEdition is null || student is null) 
            return false;
        
        return await _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(model.CourseEdition.Id, student.Id);
    }

    private static bool ValueValid(decimal arg)
    {
        return ValidValues.Contains(arg);
    }
}