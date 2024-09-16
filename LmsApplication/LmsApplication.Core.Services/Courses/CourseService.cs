using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Core.Services.Courses;

public interface ICourseService
{
    Task<List<Course>> GetAllCoursesAsync();
    
    Task<Course?> GetCourseByIdAsync(Guid id);
    
    Task<Course?> GetCourseWithEditionsByIdAsync(Guid id);
    
    Task<List<CourseCategory>> GetCategoriesAsync();
    
    Task<CourseCategory?> GetCategoryByIdAsync(Guid id);
    
    Task CreateAsync(Course course);
    
    Task DeleteAsync(Course course);
    
    Task AttachCategoriesAsync(Course course, List<Guid> categoryIds);
    
    Task CreateCategoryAsync(CourseCategory category);
    
    Task DeleteCategoryAsync(CourseCategory category);
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
        return await _dbContext.Courses
            .Include(x => x.Categories)
            .ToListAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(Guid id)
    {
        return await _dbContext.Courses
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Course?> GetCourseWithEditionsByIdAsync(Guid id)
    {
        return await _dbContext.Courses
            .Include(x => x.Categories)
            .Include(x => x.Editions)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<CourseCategory>> GetCategoriesAsync()
    {
        return await _dbContext.CourseCategories.ToListAsync();
    }

    public async Task<CourseCategory?> GetCategoryByIdAsync(Guid id)
    {
        return await _dbContext.CourseCategories.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateAsync(Course course)
    {
        await _dbContext.Courses.AddAsync(course);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course course)
    {
        course.IsDeleted = true;
        course.DeletedAtUtc = DateTime.UtcNow;
        _dbContext.Courses.Update(course);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AttachCategoriesAsync(Course course, List<Guid> categoryIds)
    {
        var categories = await _dbContext.CourseCategories.Where(x => categoryIds.Contains(x.Id)).ToListAsync();
        course.Categories = categories;
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateCategoryAsync(CourseCategory category)
    {
        await _dbContext.CourseCategories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(CourseCategory category)
    {
        category.IsDeleted = true;
        category.DeletedAtUtc = DateTime.UtcNow;
        _dbContext.CourseCategories.Update(category);
        await _dbContext.SaveChangesAsync();
    }
}