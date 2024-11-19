using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseBoardModule.Api.Controllers;

[ApiController]
[Route("api/editions/{editionId:guid}/")]
[Authorize]
public class ReactionsController : ControllerBase
{
    private readonly IReactionService _reactionService;

    public ReactionsController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    [HttpPut("posts/{postId:guid}/[controller]")]
    public async Task<IActionResult> UpdatePostReaction(Guid editionId, Guid postId, [FromQuery] ReactionType? type)
    {
        return Ok(ApiResponseHelper.Success(await _reactionService.UpdatePostReactionAsync(editionId, postId, type)));
    }

    [HttpPut("comments/{commentId:guid}/[controller]")]
    public async Task<IActionResult> UpdateCommentReaction(Guid editionId, Guid commentId, [FromQuery] ReactionType? type)
    {
        return Ok(ApiResponseHelper.Success(await _reactionService.UpdateCommentReactionAsync(editionId, commentId, type)));
    }
}