using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Services.Repositories;

public interface ICourseEditionSettingsRepository
{
    Task<CourseEditionSettings?> GetCourseEditionSettingsAsync(Guid editionId);
    
    Task AddCourseEditionSettingsAsync(CourseEditionSettings settings);
    
    Task UpdateCourseEditionSettingsAsync(CourseEditionSettings settings);
}

public class CourseEditionSettingsRepository : ICourseEditionSettingsRepository
{
    private readonly CourseDbContext _context;

    public CourseEditionSettingsRepository(CourseDbContext context)
    {
        _context = context;
    }

    public async Task<CourseEditionSettings?> GetCourseEditionSettingsAsync(Guid editionId)
    {
        return await _context.CourseEditionSettings.FirstOrDefaultAsync(x => x.CourseEditionId == editionId);
    }

    public async Task AddCourseEditionSettingsAsync(CourseEditionSettings settings)
    {
        await _context.CourseEditionSettings.AddAsync(settings);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCourseEditionSettingsAsync(CourseEditionSettings settings)
    {
        _context.CourseEditionSettings.Update(settings);
        await _context.SaveChangesAsync();
    }
}