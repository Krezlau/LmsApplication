using LmsApplication.Core.Shared.Config;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseBoardModule.Api.Controllers;

[ApiController]
[Route("api/editions/{editionId:guid}/[controller]")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }
    
    [HttpGet("current")]
    public async Task<IActionResult> GetGrades(Guid editionId)
    {
        return Ok(ApiResponseHelper.Success(await _gradeService.GetCurrentUserGradesAsync(editionId)));
    }
    
    [HttpGet("user/{userId}")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> GetGrades(Guid editionId, string userId)
    {
        return Ok(ApiResponseHelper.Success(await _gradeService.GetUserGradesAsync(editionId, userId)));
    }
    
    [HttpGet("row/{rowId:guid}")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> GetRowGrades(Guid editionId, Guid rowId)
    {
        return Ok(ApiResponseHelper.Success(await _gradeService.GetRowGradesAsync(editionId, rowId)));
    }
    
    [HttpPut("row/{rowId:guid}")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> UpdateRowValue(Guid editionId, Guid rowId, [FromQuery] string userId, UpdateRowValueModel model) 
    {
        return Ok(ApiResponseHelper.Success(await _gradeService.UpdateRowValueAsync(editionId, rowId, userId, model)));
    }
    
    [HttpDelete("row/{rowId:guid}")]
    [Authorize(AuthPolicies.TeacherPolicy)]
    public async Task<IActionResult> DeleteRowValue(Guid editionId, Guid rowId, [FromQuery] string userId) 
    {
        await _gradeService.DeleteRowValueAsync(editionId, rowId, userId);
        return Ok(ApiResponseHelper.Success());
    }
}