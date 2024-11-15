using System.Security.Claims;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseBoardModule.Api.Controllers;

[ApiController]
[Route("api/editions/{editionId:guid}/posts/{postId:guid}/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPostComments(Guid editionId, Guid postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(ApiResponseHelper.Success(await _commentService.GetCommentsForPostAsync(editionId, GetUserId(), postId, page, pageSize)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostComment(Guid editionId, Guid postId, [FromBody] CommentCreateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _commentService.CreateCommentAsync(editionId, GetUserId(), postId, model)));
    }

    [HttpPut("{commentId:guid}")]
    public async Task<IActionResult> UpdatePostComment(Guid editionId, Guid commentId, [FromBody] CommentUpdateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _commentService.UpdateCommentAsync(editionId, GetUserId(), commentId, model)));
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> DeletePostComment(Guid editionId, Guid commentId)
    {
        await _commentService.DeleteCommentAsync(editionId, GetUserId(), commentId);
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