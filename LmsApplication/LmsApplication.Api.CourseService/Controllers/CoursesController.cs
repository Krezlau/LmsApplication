using LmsApplication.Api.Shared.Controllers;
using LmsApplication.Core.ApplicationServices.Courses;
using LmsApplication.Core.Config;
using LmsApplication.Core.Data.Models;
using LmsApplication.Core.Data.Models.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.CourseService.Controllers;

[Route("api/[controller]")]
public class CoursesController : CoreController
{
    private readonly ICourseAppService _courseAppService;
    
    public CoursesController(ILogger<CoreController> logger, ICourseAppService courseAppService) : base(logger)
    {
        _courseAppService = courseAppService;
    }
    
    [HttpGet]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllCourses()
    {
        return Ok(ApiResponseHelper.Success(await _courseAppService.GetAllCoursesAsync()));
    }
    
    [HttpGet("{courseId}")]
    public async Task<IActionResult> GetCourse(Guid courseId)
    {
        return Ok(ApiResponseHelper.Success(await _courseAppService.GetCourseByIdAsync(courseId)));
    }

    [HttpPost]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCourse([FromBody] CoursePostModel model)
    {
        var courseId = await _courseAppService.CreateCourseAsync(model);

        return Ok(ApiResponseHelper.Success(courseId));
    }
    
    [HttpDelete("{courseId}")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> DeleteCourse(Guid courseId)
    {
        await _courseAppService.DeleteCourseAsync(courseId);
        
        return Ok(ApiResponseHelper.Success());
    }
    
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(ApiResponseHelper.Success(await _courseAppService.GetCategoriesAsync()));
    }
    
    [HttpPost("categories")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryPostModel model)
    {
        var category = await _courseAppService.CreateCategoryAsync(model);

        return Ok(ApiResponseHelper.Success(category));
    }
    
    [HttpDelete("categories/{categoryId}")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        await _courseAppService.DeleteCategoryAsync(categoryId);
        
        return Ok(ApiResponseHelper.Success());
    }
}