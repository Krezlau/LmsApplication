using System.Text;
using FluentValidation;

namespace LmsApplication.CourseModule.Services.Validation;

public interface IValidationService<T>
{
    Task ValidateAndThrowAsync(T instance);
    
    Task ValidateAndThrowAsync(ValidationContext<T> context);
}

public class ValidationService<T> : IValidationService<T>
{
    private readonly IValidator<T> _validator;

    public ValidationService(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async Task ValidateAndThrowAsync(T instance)
    {
        var result = await _validator.ValidateAsync(instance);
        if (result.IsValid) return;
        
        var errors = new StringBuilder();
        foreach (var error in result.Errors)
        {
            errors.AppendLine(error.ErrorMessage);
        }
        
        throw new ValidationException(errors.ToString());
    }
    
    public async Task ValidateAndThrowAsync(ValidationContext<T> context)
    {
        var result = await _validator.ValidateAsync(context);
        if (result.IsValid) return;
        
        var errors = new StringBuilder();
        foreach (var error in result.Errors)
        {
            errors.AppendLine(error.ErrorMessage);
        }
        
        throw new ValidationException(errors.ToString());
    }
}