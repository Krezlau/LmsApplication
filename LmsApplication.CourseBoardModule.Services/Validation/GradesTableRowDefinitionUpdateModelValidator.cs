using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class GradesTableRowDefinitionUpdateModelValidator : AbstractValidator<GradesTableRowDefinitionUpdateModel>
{
    public GradesTableRowDefinitionUpdateModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x)
            .Must(IsSummedValid)
            .WithMessage("Is summed can be applied only to number type rows.");
    }

    private bool IsSummedValid(GradesTableRowDefinitionUpdateModel model, GradesTableRowDefinitionUpdateModel arg2, ValidationContext<GradesTableRowDefinitionUpdateModel> context)
    {
        if (!context.RootContextData.TryGetValue(nameof(GradesTableRowDefinition), out var value) || value is not GradesTableRowDefinition row)
        {
            return false;
        }
        
        return row.RowType is RowType.Number || !model.IsSummed;
    }
}