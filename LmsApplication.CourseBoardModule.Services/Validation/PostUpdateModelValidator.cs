using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseBoardModule.Data.Models.Validation;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class PostUpdateModelValidator : AbstractValidator<UpdatePostValidationModel>
{
    public PostUpdateModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Post)
            .NotNull()
            .WithMessage("Post not found.");

        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User not found.");
        
        RuleFor(x => x)
            .Custom(UserValid);
    }

    private void UserValid(UpdatePostValidationModel model, ValidationContext<UpdatePostValidationModel> context)
    {
        if (model.Post is null || model.User is null) return;
        
        if (model.Post.UserId != model.User.Id && model.User.Role is not UserRole.Admin)
        {
            context.AddFailure("You are not allowed to update this comment.");
        }
    }
}