using LmsApplication.Api.Shared.Controllers;
using LmsApplication.Core.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.CourseService.Controllers;

[Route("api/[controller]")]
public class CoursesController : CoreController
{
    public CoursesController(ILogger<CoreController> logger) : base(logger)
    {
    }
    
    [HttpGet]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllCourses()
    {
        return Ok();
    }
}