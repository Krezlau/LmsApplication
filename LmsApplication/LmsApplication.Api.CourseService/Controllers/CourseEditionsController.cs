using LmsApplication.Api.Shared.Controllers;
using LmsApplication.Core.ApplicationServices.Courses;
using LmsApplication.Core.Config;
using LmsApplication.Core.Data.Models.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.CourseService.Controllers;

[Route("api/courses/editions")]
public class CourseEditionsController : CoreController
{
    private readonly ICourseEditionAppService _courseEditionAppService;
    
    public CourseEditionsController(ILogger<CoreController> logger, ICourseEditionAppService courseEditionAppService) : base(logger)
    {
        _courseEditionAppService = courseEditionAppService;
    }
    
    [HttpGet("all")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllCourseEditions()
    {
        return Ok(await _courseEditionAppService.GetAllCourseEditionsAsync());
    }
    
    [HttpGet("by-course/{courseId}")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetCourseEditionsByCourseId(Guid courseId)
    {
        return Ok(await _courseEditionAppService.GetCourseEditionsByCourseIdAsync(courseId));
    }
    
    [HttpPost]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCourseEdition([FromBody] CourseEditionPostModel model)
    {
        return Ok(await _courseEditionAppService.CreateCourseEditionAsync(model));
    }
    
    [HttpPost("{courseId}/add-teacher")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> AddTeacherToCourseEdition(Guid courseId, [FromBody] CourseEditionAddUserModel model)
    {
        await _courseEditionAppService.AddTeacherToCourseEditionAsync(courseId, model);
        return Ok();
    }
    
    [HttpPost("{courseId}/add-student")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> AddStudentToCourseEdition(Guid courseId, [FromBody] CourseEditionAddUserModel model)
    {
        await _courseEditionAppService.AddStudentToCourseEditionAsync(courseId, model);
        return Ok();
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseEditionById(Guid id)
    {
        return Ok(await _courseEditionAppService.GetCourseEditionByIdAsync(id));
    }
    
    [HttpGet("my-courses")]
    public async Task<IActionResult> GetMyCourses()
    {
        return Ok(await _courseEditionAppService.GetUserCourseEditionsAsync(GetUserEmail()));
    }
}