using LmsApplication.Core.Shared.Models;
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
}