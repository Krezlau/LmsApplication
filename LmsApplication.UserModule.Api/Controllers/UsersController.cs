using LmsApplication.Core.Shared.Config;
using LmsApplication.Core.Shared.Services;
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
    private readonly IUserContext _userContext;

    public UsersController(IUserService userService, IUserContext userContext)
    {
        _userService = userService;
        _userContext = userContext;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        return Ok(await _userService.GetCurrentUserAsync());
    }

    [HttpPut("current")]
    public async Task<IActionResult> UpdateCurrentUser(UserUpdateModel model)
    {
        await _userService.UpdateUserAsync(_userContext.GetUserId(), model);
        return Ok();
    }

    [HttpGet("{userEmail}")]
    public async Task<IActionResult> GetUserByEmail(string userEmail)
    {
        return Ok(await _userService.GetUserByEmailAsync(userEmail));
    }

    [HttpGet("")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _userService.GetUsersAsync(page, pageSize));
    }

    [HttpGet("by-course-edition/{courseEditionId}")]
    public async Task<IActionResult> GetUsersByCourseEdition(Guid courseEditionId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _userService.GetUsersByCourseEditionAsync(courseEditionId, page, pageSize));
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchUsersByEmail(string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _userService.SearchUsersByEmailAsync(query, page, pageSize));
    }

    [HttpPut("{userId}/role")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> UpdateUserRole(string userId, [FromBody] UpdateUserRoleModel model)
    {
        await _userService.UpdateUserRoleAsync(userId, model);
        return Ok();
    }
}