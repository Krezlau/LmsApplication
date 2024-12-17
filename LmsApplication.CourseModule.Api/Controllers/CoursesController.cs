using LmsApplication.Core.Shared.Config;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Services.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseModule.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    
    public CoursesController(ICourseService courseService) : base()
    {
        _courseService = courseService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(ApiResponseHelper.Success(await _courseService.GetAllCoursesAsync(page, pageSize)));
    }
    
    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetCourse(Guid courseId)
    {
        return Ok(ApiResponseHelper.Success(await _courseService.GetCourseByIdAsync(courseId)));
    }

    [HttpPost]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCourse([FromBody] CoursePostModel model)
    {
        var courseId = await _courseService.CreateCourseAsync(model);

        return Ok(ApiResponseHelper.Success(courseId));
    }
    
    [HttpDelete("{courseId}")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> DeleteCourse(Guid courseId)
    {
        await _courseService.DeleteCourseAsync(courseId);
        
        return Ok(ApiResponseHelper.Success());
    }
    
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(ApiResponseHelper.Success(await _courseService.GetCategoriesAsync()));
    }
    
    [HttpPost("categories")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryPostModel model)
    {
        var category = await _courseService.CreateCategoryAsync(model);

        return Ok(ApiResponseHelper.Success(category));
    }
    
    [HttpDelete("categories/{categoryId}")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        await _courseService.DeleteCategoryAsync(categoryId);
        
        return Ok(ApiResponseHelper.Success());
    }
    
    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchCourses(string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(ApiResponseHelper.Success(await _courseService.SearchCourseByNameAsync(query, page, pageSize)));
    }
}