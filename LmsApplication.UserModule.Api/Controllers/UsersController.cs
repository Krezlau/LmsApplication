using System.Security.Claims;
using LmsApplication.Core.Shared.Config;
using LmsApplication.UserModule.Data.Models;
using LmsApplication.UserModule.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.UserModule.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetUserId();

        return Ok(await _userService.GetUserAsync(userId));
    }
    
    [HttpPut("current")]
    public async Task<IActionResult> UpdateCurrentUser(UserUpdateModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            throw new ArgumentException("Invalid user.");
        }

        await _userService.UpdateUserAsync(userId, model);
        return Ok();
    }

    [HttpGet("{userEmail}")]
    public async Task<IActionResult> GetUserByEmail(string userEmail)
    {
        return Ok(await _userService.GetUserByEmailAsync(userEmail));
    }
    
    [HttpGet("")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await _userService.GetUsersAsync());
    }
    
    [HttpGet("by-course-edition/{courseEditionId}")]
    public async Task<IActionResult> GetUsersByCourseEdition(Guid courseEditionId)
    {
        var userId = GetUserId();
        
        return Ok(await _userService.GetUsersByCourseEditionAsync(courseEditionId, userId));
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchUsersByEmail(string query)
    {
        return Ok(await _userService.SearchUsersByEmailAsync(query));
    }
    
    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            throw new ArgumentException("Invalid user.");
        }

        return userId;
    }
}