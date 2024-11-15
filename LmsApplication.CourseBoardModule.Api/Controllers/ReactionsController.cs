using System.Security.Claims;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseBoardModule.Api.Controllers;

[ApiController]
[Route("api/editions/{editionId:guid}/[action]/[controller]")]
[Authorize]
public class ReactionsController : ControllerBase
{
    private readonly IReactionService _reactionService;

    public ReactionsController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    [HttpPut("posts/{postId:guid}")]
    public async Task<IActionResult> UpdatePostReaction(Guid editionId, Guid postId, [FromBody] ReactionUpdateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _reactionService.UpdatePostReactionAsync(editionId, GetUserId(), postId, model)));
    }

    [HttpPut("comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateCommentReaction(Guid editionId, Guid commentId, [FromBody] ReactionUpdateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _reactionService.UpdateCommentReactionAsync(editionId, GetUserId(), commentId, model)));
    }
    
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) 
            throw new ArgumentException("Incorrect user id.");
        
        return userId;
    }
}