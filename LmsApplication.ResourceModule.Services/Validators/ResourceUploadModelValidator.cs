using FluentValidation;
using LmsApplication.ResourceModule.Data.Entities;
using LmsApplication.ResourceModule.Data.Models;
using LmsApplication.ResourceModule.Services.Providers;

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
        
        // max file size 512MB
        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(512 * 1024 * 1024)
            .WithMessage("File size can't exceed 512MB");

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