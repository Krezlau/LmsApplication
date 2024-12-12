using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Models.Validation;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class PostCreateModelValidator : AbstractValidator<CreatePostValidationModel>
{
    public PostCreateModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User not found.");
    }
}