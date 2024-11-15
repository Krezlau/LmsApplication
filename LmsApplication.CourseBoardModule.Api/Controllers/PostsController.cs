using System.Security.Claims;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.CourseBoardModule.Api.Controllers;

[ApiController]
[Route("api/editions/{editionId:guid}/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPosts(Guid editionId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(ApiResponseHelper.Success(await _postService.GetPostsAsync(editionId, GetUserId(), page, pageSize)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePost(Guid editionId, [FromBody] PostCreateModel model)
    {
        return Ok(ApiResponseHelper.Success(await _postService.CreatePostAsync(editionId, GetUserId(), model)));
    }
    
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) 
            throw new ArgumentException("Incorrect user id.");
        
        return userId;
    }
}