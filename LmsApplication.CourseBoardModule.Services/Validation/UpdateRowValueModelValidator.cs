using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class UpdateRowValueModelValidator : AbstractValidator<UpdateRowValueModel>
{
    public UpdateRowValueModelValidator()
    {
        RuleFor(x => x.Value)
            .NotNull()
            .Must(ValueValid)
            .WithMessage("Value is not valid.");
    }

    private static bool ValueValid(UpdateRowValueModel model, string value, ValidationContext<UpdateRowValueModel> context)
    {
        if (!context.RootContextData.TryGetValue(nameof(GradesTableRowDefinition), out var rowDefinitionObject) ||
            rowDefinitionObject is not GradesTableRowDefinition rowDefinition)
        {
            return false;
        }
        
        return rowDefinition.RowType switch
        {
            RowType.Text => true,
            RowType.Number => decimal.TryParse((string)value, out _),
            RowType.Bool => bool.TryParse((string)value, out _),
            RowType.None => false,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}