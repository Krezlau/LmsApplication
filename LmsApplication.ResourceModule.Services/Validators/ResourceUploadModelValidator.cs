using FluentValidation;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.ResourceModule.Data.Entities;
using LmsApplication.ResourceModule.Data.Models;

namespace LmsApplication.ResourceModule.Services.Validators;

public class ResourceUploadModelValidator : AbstractValidator<ResourceUploadModel>
{
    private readonly ICourseEditionProvider _courseEditionProvider;
    private readonly ICourseProvider _courseProvider;
    
    public ResourceUploadModelValidator(ICourseEditionProvider courseEditionProvider, ICourseProvider courseProvider)
    {
        _courseEditionProvider = courseEditionProvider;
        _courseProvider = courseProvider;

        RuleFor(x => x.FileDisplayName)
            .Matches(@"^[a-zA-Z0-9_-]+$");
        
        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(104857600);

        RuleFor(x => x)
            .MustAsync(ParentExistsAsync)
            .WithMessage("Parent doesn't exist");
    }

    private async Task<bool> ParentExistsAsync(ResourceUploadModel model, CancellationToken ct)
    {
        switch (model.Type)
        {
            case ResourceType.Edition:
                return await _courseEditionProvider.CourseEditionExistsAsync(model.ParentId);
            case ResourceType.Course:
                return await _courseProvider.CourseExistsAsync(model.ParentId);
        }

        return false;
    }
}