using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Mapping;
using LmsApplication.Core.Data.Models;
using LmsApplication.Core.Services.Courses;

namespace LmsApplication.Core.ApplicationServices.Courses;

public interface ICourseAppService
{
    Task<List<CourseModel>> GetAllCoursesAsync();
    
    Task<Guid> CreateCourseAsync(CoursePostModel courseModel);
}

public class CourseAppService : ICourseAppService
{
    private readonly ICourseService _courseService;

    public CourseAppService(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public async Task<List<CourseModel>> GetAllCoursesAsync()
    {
        var courses = await _courseService.GetAllCoursesAsync();

        return courses.Select(x => x.ToModel()).ToList();
    }

    public async Task<Guid> CreateCourseAsync(CoursePostModel courseModel)
    {
        var course = new Course
        {
            Title = courseModel.Title,
            Description = courseModel.Description,
            Categories = courseModel.Categories,
            Duration = courseModel.Duration,
        };

        await _courseService.UpsertAsync(course);
        
        return course.Id;
    }
}