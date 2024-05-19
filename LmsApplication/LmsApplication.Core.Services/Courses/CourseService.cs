using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Core.Services.Courses;

public interface ICourseService
{
    Task<List<Course>> GetAllCoursesAsync();
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
        return await _dbContext.Courses.ToListAsync();
    }
}