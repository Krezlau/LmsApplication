using FluentValidation;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Services.Validation;

public class PostCreateModelValidator : AbstractValidator<PostCreateModel>
{
    public PostCreateModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty();
    }
}