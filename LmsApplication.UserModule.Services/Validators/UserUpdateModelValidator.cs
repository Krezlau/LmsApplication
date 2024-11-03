using FluentValidation;
using LmsApplication.UserModule.Data.Models;

namespace LmsApplication.UserModule.Services.Validators;

public class UserUpdateModelValidator : AbstractValidator<UserUpdateModel>
{
    public UserUpdateModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);
        
        RuleFor(x => x.Surname)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);
        
        RuleFor(x => x.Bio)
            .MaximumLength(500);
    }
}