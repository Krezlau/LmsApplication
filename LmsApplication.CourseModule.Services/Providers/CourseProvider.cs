using LmsApplication.Core.Shared.Providers;
using LmsApplication.CourseModule.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Services.Providers;

public class CourseProvider : ICourseProvider
{
    private readonly CourseDbContext _context;

    public CourseProvider(CourseDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CourseExistsAsync(Guid courseId)
    {
        return await _context.Courses.AnyAsync(x => x.Id == courseId);
    }
}