using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Core.Services.Courses;

public interface ICourseService
{
    Task<List<Course>> GetAllCoursesAsync();
    
    Task<Course?> GetCourseByIdAsync(Guid id);
    
    Task<List<Course>> GetCoursesByIdsAsync(List<Guid> ids);
    
    Task CreateAsync(Course course);
    
    Task UpdateAsync(Course course);
}

public class CourseService : ICourseService
{
    private readonly CourseDbContext _dbContext;

    public CourseService(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        // todo pagination
        return await _dbContext.Courses.ToListAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(Guid id)
    {
        return await _dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Course>> GetCoursesByIdsAsync(List<Guid> ids)
    {
        return await _dbContext.Courses.Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task CreateAsync(Course course)
    {
        await _dbContext.Courses.AddAsync(course);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _dbContext.Courses.Update(course);
        await _dbContext.SaveChangesAsync();
    }
}