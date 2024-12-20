using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Services.Repositories;

public interface ICourseRepository
{
    Task<(int totalCount, List<Course> courses)> GetAllCoursesAsync(int page, int pageSize);
    
    Task<Course?> GetCourseByIdAsync(Guid id);
    
    Task<Course?> GetCourseWithEditionsByIdAsync(Guid id);
    
    Task<(int totalCount, List<Course> courses)> SearchCoursesByName(string query, int page, int pageSize);
    
    Task<List<CourseCategory>> GetCategoriesAsync();
    
    Task<CourseCategory?> GetCategoryByIdAsync(Guid id);
    
    Task<bool> CourseExistsAsync(Guid courseId);
    
    Task CreateAsync(Course course);
    
    Task DeleteAsync(Course course);
    
    Task AttachCategoriesAsync(Course course, List<Guid> categoryIds);
    
    Task CreateCategoryAsync(CourseCategory category);
    
    Task DeleteCategoryAsync(CourseCategory category);
}

public class CourseRepository : ICourseRepository
{
    private readonly CourseDbContext _dbContext;

    public CourseRepository(CourseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int totalCount, List<Course> courses)> GetAllCoursesAsync(int page, int pageSize)
    {
        var query = _dbContext.Courses
            .Include(x => x.Categories);
        
        var totalCount = await query.CountAsync();
        var courses = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, courses);
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

    public async Task<(int totalCount, List<Course> courses)> SearchCoursesByName(string query, int page, int pageSize)
    {
        var q = _dbContext.Courses
            .Include(x => x.Categories)
            .Where(x => x.Title.Contains(query));
        
        var totalCount = await q.CountAsync();
        var courses = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, courses);
    }

    public async Task<List<CourseCategory>> GetCategoriesAsync()
    {
        return await _dbContext.CourseCategories.ToListAsync();
    }

    public async Task<CourseCategory?> GetCategoryByIdAsync(Guid id)
    {
        return await _dbContext.CourseCategories.FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<bool> CourseExistsAsync(Guid courseId)
    {
        return await _dbContext.Courses.AnyAsync(x => x.Id == courseId);
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