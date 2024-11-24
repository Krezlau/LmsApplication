using LmsApplication.Core.Shared.Config;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseBoardModule.Api.Controllers;

[ApiController]
[Route("api/editions/{editionId:guid}/grades/rows")]
[Authorize(AuthPolicies.TeacherPolicy)]
public class GradesTableRowDefinitionsController : ControllerBase
{
    private readonly IGradesTableRowDefinitionService _gradesTableRowDefinitionService;

    public GradesTableRowDefinitionsController(IGradesTableRowDefinitionService gradesTableRowDefinitionService)
    {
        _gradesTableRowDefinitionService = gradesTableRowDefinitionService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetGradesTableRowDefinitions(Guid editionId)
    {
        return Ok(ApiResponseHelper.Success(await _gradesTableRowDefinitionService.GetGradesTableRowDefinitionsAsync(editionId)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateGradesTableRowDefinition(Guid editionId, [FromBody] GradesTableRowDefinitionCreateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _gradesTableRowDefinitionService.CreateGradesTableRowDefinitionAsync(editionId, model)));
    }
    
    [HttpPut("{rowId:guid}")]
    public async Task<IActionResult> UpdateGradesTableRowDefinition(Guid editionId, Guid rowId, [FromBody] GradesTableRowDefinitionUpdateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _gradesTableRowDefinitionService.UpdateGradesTableRowDefinitionAsync(editionId, rowId, model)));
    }
    
    [HttpDelete("{rowId:guid}")]
    public async Task<IActionResult> DeleteGradesTableRowDefinition(Guid editionId, Guid rowId)
    {
        await _gradesTableRowDefinitionService.DeleteGradesTableRowDefinitionAsync(editionId, rowId);
        return Ok(ApiResponseHelper.Success());
    }
}