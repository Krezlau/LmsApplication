using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class CommentCreateModelValidator : AbstractValidator<CommentCreateModel>
{
    public CommentCreateModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);
    }
}