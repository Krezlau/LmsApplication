using FluentValidation;
using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Models.Courses;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Api.Shared.Validation;

public class CategoryPostModelValidator : AbstractValidator<CategoryPostModel>
{
    private const string CategoryAlreadyExistsMessage = "Category with this name already exists.";
    
    private readonly CourseDbContext _context;
    
    public CategoryPostModelValidator(CourseDbContext context)
    {
        _context = context;
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Name)
            .MustAsync(IsUniqueNameAsync)
            .WithMessage(CategoryAlreadyExistsMessage);
    }

    private async Task<bool> IsUniqueNameAsync(string name, CancellationToken ct)
    {
        return await _context.CourseCategories
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken: ct) is null;
    }
}