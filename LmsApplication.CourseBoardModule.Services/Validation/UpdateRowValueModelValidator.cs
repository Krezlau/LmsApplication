using FluentValidation;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class UpdateRowValueModelValidator : AbstractValidator<UpdateRowValueValidationModel>
{
    private readonly ICourseEditionProvider _courseEditionProvider;
    private readonly IFinalGradeRepository _finalGradeRepository;
    
    public UpdateRowValueModelValidator(ICourseEditionProvider courseEditionProvider, IFinalGradeRepository finalGradeRepository)
    {
        _courseEditionProvider = courseEditionProvider;
        _finalGradeRepository = finalGradeRepository;

        RuleFor(x => x.Teacher)
            .NotNull()
            .WithMessage("Could not find teacher.");
        
        RuleFor(x => x.Student)
            .NotNull()
            .WithMessage("Could not find student.");

        RuleFor(x => x.Student)
            .MustAsync(StudentEnrolledAsync)
            .WithMessage("Student is not enrolled in this course edition.");
        
        RuleFor(x => x.Student) 
            .MustAsync(StudentDoesNotHaveFinalGradeAsync)
            .WithMessage("Cannot update row value for student with final grade.");
        
        RuleFor(x => x.RowDefinition)
            .NotNull()
            .WithMessage("Could not find row definition.");
        
        RuleFor(x => x.Value)
            .NotNull()
            .Must(ValueValid)
            .WithMessage("Value is not valid.");
    }

    private async Task<bool> StudentDoesNotHaveFinalGradeAsync(UpdateRowValueValidationModel model, UserExchangeModel? student, CancellationToken ct)
    {
        if (model.CourseEdition is null || student is null) return false;
        return !await _finalGradeRepository.GradeExistsAsync(model.CourseEdition.Id, student.Id);
    }

    private async Task<bool> StudentEnrolledAsync(UpdateRowValueValidationModel model, UserExchangeModel? student, CancellationToken ct)
    {
        if (model.CourseEdition is null || student is null) return false;
        return await _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(model.CourseEdition.Id, student.Id);
    }

    private static bool ValueValid(UpdateRowValueValidationModel model, string value, ValidationContext<UpdateRowValueValidationModel> context)
    {
        return model.RowDefinition!.RowType switch
        {
            RowType.Text => true,
            RowType.Number => decimal.TryParse(value, out _),
            RowType.Bool => bool.TryParse(value, out _),
            RowType.None => false,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}