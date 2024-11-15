using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class CommentUpdateModelValidator : AbstractValidator<CommentUpdateModel>
{
    public CommentUpdateModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x)
            .Custom(UserValid);
    }

    private void UserValid(CommentUpdateModel model, ValidationContext<CommentUpdateModel> context)
    {
        if (!context.RootContextData.TryGetValue(nameof(Comment), out var value) || value is not Comment comment)
        {
            context.AddFailure("Comment not found.");
            return;
        }
        
        if (!context.RootContextData.TryGetValue("user", out var userVal) || userVal is not UserExchangeModel user)
        {
            context.AddFailure("User not found.");
            return;
        }
        
        if (comment.UserId != user.Id && user.Role is not UserRole.Admin)
        {
            context.AddFailure("You are not allowed to update this comment.");
        }
    }
}