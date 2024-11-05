using System.Security.Claims;
using LmsApplication.Core.Shared.Config;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Services.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseModule.Api.Controllers;

[ApiController]
[Route("api/courses/editions")]
[Authorize]
public class CourseEditionsController : ControllerBase
{
    private readonly ICourseEditionService _courseEditionService;

    public CourseEditionsController(ICourseEditionService courseEditionService) : base()
    {
        _courseEditionService = courseEditionService;
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllCourseEditions()
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionService.GetAllCourseEditionsAsync()));
    }
    
    [HttpGet("by-course/{courseId}")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetCourseEditionsByCourseId(Guid courseId)
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionService.GetCourseEditionsByCourseIdAsync(courseId)));
    }
    
    [HttpPost]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCourseEdition([FromBody] CourseEditionPostModel model)
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionService.CreateCourseEditionAsync(model)));
    }
    
    [HttpPost("{courseEditionId}/register")]
    public async Task<IActionResult> RegisterToCourseEdition(Guid courseEditionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) 
            return BadRequest(ApiResponseHelper.Error("User not found."));
        
        await _courseEditionService.RegisterToCourseEditionAsync(courseEditionId, userId);
        return Ok(ApiResponseHelper.Success());
    }
    
    [HttpPost("{courseEditionId}/add-user")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> AddUserToCourseEdition(Guid courseEditionId, [FromBody] CourseEditionAddUserModel model)
    {
        await _courseEditionService.AddUserToCourseEditionAsync(courseEditionId, model);
        return Ok(ApiResponseHelper.Success());
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseEditionById(Guid id)
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionService.GetCourseEditionByIdAsync(id)));
    }
    
    [HttpGet("my-courses")]
    public async Task<IActionResult> GetMyCourses()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) 
            return BadRequest(ApiResponseHelper.Error("User not found."));
        
        return Ok(ApiResponseHelper.Success(await _courseEditionService.GetUserCourseEditionsAsync(userId)));
    }
}