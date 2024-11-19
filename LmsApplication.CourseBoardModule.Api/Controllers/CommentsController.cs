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
        return Ok(ApiResponseHelper.Success(await _commentService.GetCommentsForPostAsync(editionId, postId, page, pageSize)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostComment(Guid editionId, Guid postId, [FromBody] CommentCreateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _commentService.CreateCommentAsync(editionId, postId, model)));
    }

    [HttpPut("{commentId:guid}")]
    public async Task<IActionResult> UpdatePostComment(Guid editionId, Guid commentId, [FromBody] CommentUpdateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _commentService.UpdateCommentAsync(editionId, commentId, model)));
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> DeletePostComment(Guid editionId, Guid commentId)
    {
        await _commentService.DeleteCommentAsync(editionId, commentId);
        return Ok(ApiResponseHelper.Success());
    }
}