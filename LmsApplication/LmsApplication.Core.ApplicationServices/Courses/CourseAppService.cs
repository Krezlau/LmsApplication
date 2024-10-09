using LmsApplication.Api.Shared.Validation;
using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Mapping;
using LmsApplication.Core.Data.Models.Courses;
using LmsApplication.Core.Services.Courses;

namespace LmsApplication.Core.ApplicationServices.Courses;

public interface ICourseAppService
{
    Task<List<CourseModel>> GetAllCoursesAsync();
    
    Task<CourseModel> GetCourseByIdAsync(Guid id);
    
    Task<List<CourseCategoryModel>> GetCategoriesAsync();
    
    Task<CourseModel> CreateCourseAsync(CoursePostModel courseModel);
    
    Task DeleteCourseAsync(Guid courseId);
    
    Task<CourseCategoryModel> CreateCategoryAsync(CategoryPostModel categoryModel);
    
    Task DeleteCategoryAsync(Guid categoryId);
}

public class CourseAppService : ICourseAppService
{
    private readonly ICourseService _courseService;
    private readonly IValidationService<CoursePostModel> _coursePostModelValidationService;
    private readonly IValidationService<CategoryPostModel> _categoryPostModelValidationService;

    public CourseAppService(ICourseService courseService,
        IValidationService<CategoryPostModel> categoryPostModelValidationService,
        IValidationService<CoursePostModel> coursePostModelValidationService)
    {
        _courseService = courseService;
        _categoryPostModelValidationService = categoryPostModelValidationService;
        _coursePostModelValidationService = coursePostModelValidationService;
    }

    public async Task<List<CourseModel>> GetAllCoursesAsync()
    {
        var courses = await _courseService.GetAllCoursesAsync();

        return courses.Select(x => x.ToModel()).ToList();
    }

    public async Task<CourseModel> GetCourseByIdAsync(Guid id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course is null)
            throw new KeyNotFoundException($"{nameof(Course)} not found.");

        return course.ToModel();
    }

    public async Task<List<CourseCategoryModel>> GetCategoriesAsync()
    {
        var categories = await _courseService.GetCategoriesAsync();
        
        return categories.Select(x => x.ToModel()).ToList();
    }

    public async Task<CourseModel> CreateCourseAsync(CoursePostModel courseModel)
    {
        await _coursePostModelValidationService.ValidateAndThrowAsync(courseModel);
        
        var course = new Course
        {
            Title = courseModel.Title,
            Description = courseModel.Description,
            Duration = courseModel.Duration,
        };

        await _courseService.CreateAsync(course);
        
        await _courseService.AttachCategoriesAsync(course, courseModel.Categories);
        
        return course.ToModel();
    }

    public async Task DeleteCourseAsync(Guid courseId)
    {
        var course = await _courseService.GetCourseWithEditionsByIdAsync(courseId);
        
        if (course is null)
            throw new KeyNotFoundException($"{nameof(Course)} not found.");
        
        if (course.Editions.Count != 0)
            throw new InvalidOperationException("Cannot delete course with editions.");
        
        await _courseService.DeleteAsync(course);
    }

    public async Task<CourseCategoryModel> CreateCategoryAsync(CategoryPostModel categoryModel)
    {
        await _categoryPostModelValidationService.ValidateAndThrowAsync(categoryModel);
        
        var category = new CourseCategory
        {
            Name = categoryModel.Name,
        };

        await _courseService.CreateCategoryAsync(category);

        return category.ToModel();
    }

    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        var category = await _courseService.GetCategoryByIdAsync(categoryId);
        
        if (category is null)
            throw new KeyNotFoundException($"{nameof(CourseCategory)} not found.");
        
        await _courseService.DeleteCategoryAsync(category);
    }
}