using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Providers;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class GradesTableRowDefinitionCreateModelValidator: AbstractValidator<GradesTableRowDefinitionCreateModel>
{
    private readonly ICourseEditionProvider _courseEditionProvider;
    
    public GradesTableRowDefinitionCreateModelValidator(ICourseEditionProvider courseEditionProvider)
    {
        _courseEditionProvider = courseEditionProvider;
        
        RuleFor(x => x.CourseEditionId)
            .MustAsync(CourseEditionExistsAsync)
            .WithMessage("Course edition does not exist.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x)
            .Must(IsSummedValid)
            .WithMessage("Is summed can be applied only to number type rows.");
    }

    private static bool IsSummedValid(GradesTableRowDefinitionCreateModel model)
    {
        return model.RowType is RowType.Number || !model.IsSummed;
    }

    private Task<bool> CourseEditionExistsAsync(Guid editionId, CancellationToken ct)
    {
        return _courseEditionProvider.CourseEditionExistsAsync(editionId);
    }
}