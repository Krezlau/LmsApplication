using LmsApplication.Api.Shared.Controllers;
using LmsApplication.Core.ApplicationServices.Courses;
using LmsApplication.Core.Config;
using LmsApplication.Core.Data.Models;
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
        return Ok(await _courseAppService.GetAllCoursesAsync());
    }

    [HttpPost]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> CreateCourse([FromBody] CoursePostModel model)
    {
        var courseId = await _courseAppService.CreateCourseAsync(model);

        return Ok(courseId);
    }
}