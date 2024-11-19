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
        return Ok(ApiResponseHelper.Success(await _courseEditionSettingsService.GetCourseEditionSettingsAsync(editionId)));
    }
    
    [HttpPut]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> UpdateCourseEditionSettings(Guid editionId, [FromBody] CourseEditionSettingsUpdateModel model)
    {
        await _courseEditionSettingsService.UpdateCourseEditionSettingsAsync(editionId, model);
        return Ok(ApiResponseHelper.Success());
    }
}