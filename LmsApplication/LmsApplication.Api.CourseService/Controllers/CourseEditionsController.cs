using LmsApplication.Api.Shared.Controllers;
using LmsApplication.Core.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.CourseService.Controllers;

[Route("api/courses/editions")]
public class CourseEditionsController : CoreController
{
    public CourseEditionsController(ILogger<CoreController> logger) : base(logger)
    {
    }
    
    [HttpGet("all")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllCourseEditions()
    {
        return Ok();
    }
    
    [HttpPost]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCourseEdition()
    {
        return Ok();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseEditionById(Guid id)
    {
        return Ok();
    }
    
    [HttpGet("my-courses")]
    public async Task<IActionResult> GetMyCourses()
    {
        return Ok();
    }
}