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

    private static bool ValueValid(UpdateRowValueModel model, object value, ValidationContext<UpdateRowValueModel> context)
    {
        if (!context.RootContextData.TryGetValue(nameof(GradesTableRowValue), out var rowValueObject) || rowValueObject is not GradesTableRowValue rowValue)
        {
            return false;
        }
        
        return rowValue.RowDefinition.RowType switch
        {
            RowType.Text => value is string,
            RowType.Number => value is int,
            RowType.Bool => value is bool,
            RowType.None => false,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}