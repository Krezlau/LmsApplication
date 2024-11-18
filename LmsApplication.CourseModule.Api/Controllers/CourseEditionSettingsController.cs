using System.Security.Claims;
using LmsApplication.Core.Shared.Config;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Services.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseModule.Api.Controllers;

[ApiController]
[Route("api/editions/{editionId:guid}/settings")]
[Authorize]
public class CourseEditionSettingsController : ControllerBase
{
    private readonly ICourseEditionSettingsService _courseEditionSettingsService;

    public CourseEditionSettingsController(ICourseEditionSettingsService courseEditionSettingsService)
    {
        _courseEditionSettingsService = courseEditionSettingsService;
    }

    [HttpGet]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> GetCourseEditionSettings(Guid editionId)
    {
        return Ok(ApiResponseHelper.Success(await _courseEditionSettingsService.GetCourseEditionSettingsAsync(editionId, GetUserId())));
    }
    
    [HttpPut]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> UpdateCourseEditionSettings(Guid editionId, [FromBody] CourseEditionSettingsUpdateModel model)
    {
        await _courseEditionSettingsService.UpdateCourseEditionSettingsAsync(editionId, GetUserId(), model);
        return Ok(ApiResponseHelper.Success());
    }
    
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) 
            throw new ArgumentException("Incorrect user id.");
        
        return userId;
    }
}