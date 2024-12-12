using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models.Validation;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class GradesTableRowDefinitionUpdateModelValidator : AbstractValidator<UpdateRowDefinitionValidationModel>
{
    public GradesTableRowDefinitionUpdateModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.RowDefinition)
            .NotNull()
            .WithMessage("Row definition not found.");
        
        RuleFor(x => x)
            .Must(IsSummedValid)
            .WithMessage("Is summed can be applied only to number type rows.");
    }

    private bool IsSummedValid(UpdateRowDefinitionValidationModel model, UpdateRowDefinitionValidationModel arg2, ValidationContext<UpdateRowDefinitionValidationModel> context)
    {
        if (model.RowDefinition is null)
        {
            return false;
        }
        
        return model.RowDefinition.RowType is RowType.Number || !model.IsSummed;
    }
}