using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseBoardModule.Data.Models.Validation;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class CommentUpdateModelValidator : AbstractValidator<UpdateCommentValidationModel>
{
    public CommentUpdateModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Comment)
            .NotNull()
            .WithMessage("Comment not found.");
        
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User not found.");
        
        RuleFor(x => x)
            .Custom(UserValid);
    }

    private static void UserValid(UpdateCommentValidationModel model, ValidationContext<UpdateCommentValidationModel> context)
    {
        if (model.Comment is null || model.User is null) return;
        
        if (model.Comment.UserId != model.User.Id && model.User.Role is not UserRole.Admin)
        {
            context.AddFailure("You are not allowed to update this comment.");
        }
    }
}