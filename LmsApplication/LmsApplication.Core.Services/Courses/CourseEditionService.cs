using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Core.Services.Courses;

public interface ICourseEditionService
{
    Task<List<CourseEdition>> GetAllCourseEditionsAsync();
    
    Task<List<CourseEdition>> GetCourseEditionsByCourseIdAsync(Guid courseId);
    
    Task<List<CourseEdition>> GetUserCourseEditionsAsync(string userEmail);
    
    Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id);

    Task UpsertAsync(CourseEdition courseEdition);
}

public class CourseEditionService : ICourseEditionService
{
    private readonly CourseDbContext _context;

    public CourseEditionService(CourseDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseEdition>> GetAllCourseEditionsAsync()
    {
        return await _context.CourseEditions.ToListAsync();
    }

    public async Task<List<CourseEdition>> GetCourseEditionsByCourseIdAsync(Guid courseId)
    {
        return await _context.CourseEditions.Where(x => x.CourseId == courseId).ToListAsync();
    }

    public async Task<List<CourseEdition>> GetUserCourseEditionsAsync(string userEmail)
    {
        var courseEditions = await _context.CourseEditions
            .Where(x => x.EndDateUtc < DateTime.UtcNow)
            .ToListAsync();
        
        return courseEditions.Where(x => x.StudentEmails.Contains(userEmail) || x.TeacherEmails.Contains(userEmail)).ToList();
    }

    public async Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id)
    {
        return await _context.CourseEditions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpsertAsync(CourseEdition courseEdition)
    {
        _context.CourseEditions.Update(courseEdition);
        await _context.SaveChangesAsync();
    }
}