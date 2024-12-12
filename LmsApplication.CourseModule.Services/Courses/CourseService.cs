using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.CourseModule.Data.Mapping;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Courses;

public interface ICourseService
{
    Task<List<CourseModel>> GetAllCoursesAsync();
    
    Task<CourseModel> GetCourseByIdAsync(Guid id);
    
    Task<List<CourseCategoryModel>> GetCategoriesAsync();
    
    Task<CourseModel> CreateCourseAsync(CoursePostModel courseModel);
    
    Task DeleteCourseAsync(Guid courseId);
    
    Task<CourseCategoryModel> CreateCategoryAsync(CategoryPostModel categoryModel);
    
    Task DeleteCategoryAsync(Guid categoryId);
}

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IValidationService<CoursePostModel> _coursePostModelValidationService;
    private readonly IValidationService<CategoryPostModel> _categoryPostModelValidationService;

    public CourseService(ICourseRepository courseRepository,
        IValidationService<CategoryPostModel> categoryPostModelValidationService,
        IValidationService<CoursePostModel> coursePostModelValidationService)
    {
        _courseRepository = courseRepository;
        _categoryPostModelValidationService = categoryPostModelValidationService;
        _coursePostModelValidationService = coursePostModelValidationService;
    }

    public async Task<List<CourseModel>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();

        return courses.Select(x => x.ToModel()).ToList();
    }

    public async Task<CourseModel> GetCourseByIdAsync(Guid id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
        if (course is null)
            throw new KeyNotFoundException($"{nameof(Course)} not found.");

        return course.ToModel();
    }

    public async Task<List<CourseCategoryModel>> GetCategoriesAsync()
    {
        var categories = await _courseRepository.GetCategoriesAsync();
        
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

        await _courseRepository.CreateAsync(course);
        
        await _courseRepository.AttachCategoriesAsync(course, courseModel.Categories);
        
        return course.ToModel();
    }

    public async Task DeleteCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetCourseWithEditionsByIdAsync(courseId);
        
        if (course is null)
            throw new KeyNotFoundException($"{nameof(Course)} not found.");
        
        if (course.Editions.Count != 0)
            throw new InvalidOperationException("Cannot delete course with editions.");
        
        await _courseRepository.DeleteAsync(course);
    }

    public async Task<CourseCategoryModel> CreateCategoryAsync(CategoryPostModel categoryModel)
    {
        await _categoryPostModelValidationService.ValidateAndThrowAsync(categoryModel);
        
        var category = new CourseCategory
        {
            Name = categoryModel.Name,
        };

        await _courseRepository.CreateCategoryAsync(category);

        return category.ToModel();
    }

    public async Task DeleteCategoryAsync(Guid categoryId)
    {
        var category = await _courseRepository.GetCategoryByIdAsync(categoryId);
        
        if (category is null)
            throw new KeyNotFoundException($"{nameof(CourseCategory)} not found.");
        
        await _courseRepository.DeleteCategoryAsync(category);
    }
}