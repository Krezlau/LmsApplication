using LmsApplication.Core.Data.Config;
using LmsApplication.Core.Data.Models;
using LmsApplication.Core.Data.Models.Courses;
using LmsApplication.CourseModule.Services.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseModule.Api.Controllers;

[ApiController]
[Route("api/courses/editions")]
[Authorize]
public class CourseEditionsController : ControllerBase
{
    private readonly ICourseEditionAppService _courseEditionAppService;

    public CourseEditionsController(ICourseEditionAppService courseEditionAppService) : base()
    {
        _courseEditionAppService = courseEditionAppService;
    }
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllCourseEditions()
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionAppService.GetAllCourseEditionsAsync()));
    }
    
    [HttpGet("by-course/{courseId}")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetCourseEditionsByCourseId(Guid courseId)
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionAppService.GetCourseEditionsByCourseIdAsync(courseId)));
    }
    
    [HttpPost]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCourseEdition([FromBody] CourseEditionPostModel model)
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionAppService.CreateCourseEditionAsync(model)));
    }
    
    [HttpPost("{courseId}/add-teacher")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> AddTeacherToCourseEdition(Guid courseId, [FromBody] CourseEditionAddUserModel model)
    {
        await _courseEditionAppService.AddTeacherToCourseEditionAsync(courseId, model);
        return Ok(ApiResponseHelper.Success());
    }
    
    [HttpPost("{courseId}/add-student")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> AddStudentToCourseEdition(Guid courseId, [FromBody] CourseEditionAddUserModel model)
    {
        await _courseEditionAppService.AddStudentToCourseEditionAsync(courseId, model);
        return Ok(ApiResponseHelper.Success());
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseEditionById(Guid id)
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionAppService.GetCourseEditionByIdAsync(id)));
    }
    //
    // [HttpGet("my-courses")]
    // public async Task<IActionResult> GetMyCourses()
    // {
    //     return Ok(ApiResponseHelper.Success(await _courseEditionAppService.GetUserCourseEditionsAsync(GetUserEmail())));
    // }
}