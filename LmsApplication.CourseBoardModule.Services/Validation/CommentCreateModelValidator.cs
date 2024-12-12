using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Models.Validation;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class CommentCreateModelValidator : AbstractValidator<CreateCommentValidationModel>
{
    public CommentCreateModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);
        
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User not found");
        
        RuleFor(x => x.Post)
            .NotNull()
            .WithMessage("Post not found");
    }
}