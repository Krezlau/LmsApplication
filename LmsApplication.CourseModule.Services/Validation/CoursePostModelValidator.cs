using FluentValidation;
using LmsApplication.Core.Data.Models.Courses;
using LmsApplication.CourseModule.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Services.Validation;

public class CoursePostModelValidator : AbstractValidator<CoursePostModel>
{
    private const string TitleNotValidMessage = "Title must be between 3 and 100 characters.";
    private const string DescriptionNotValidMessage = "Description must be at least 3 characters long.";
    private const string CategoriesNotValidMessage = "Categories must exist.";
    
    private readonly CourseDbContext _context;
    
    public CoursePostModelValidator(CourseDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage(TitleNotValidMessage);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage(DescriptionNotValidMessage);

        RuleFor(x => x.Categories)
            .MustAsync(CategoriesExistAsync)
            .When(x => x.Categories.Count > 0)
            .WithMessage(CategoriesNotValidMessage);
    }

    private async Task<bool> CategoriesExistAsync(List<Guid> categoryIds, CancellationToken ct)
    {
        var count = await _context.CourseCategories.Where(x => categoryIds.Contains(x.Id)).CountAsync(cancellationToken: ct);
        
        return count == categoryIds.Count;
    }
}