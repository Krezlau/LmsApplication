using LmsApplication.Api.Shared.Controllers;
using LmsApplication.Core.ApplicationServices.Users;
using LmsApplication.Core.Config;
using LmsApplication.Core.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.AuthService.Controllers;

[Route("api/[controller]")]
public class AuthController : CoreController
{
    private readonly IUserAppService _userAppService;

    public AuthController(ILogger<AuthController> logger, IUserAppService userAppService) : base(logger)
    {
        _userAppService = userAppService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userEmail = GetUserEmail();
        var user = await _userAppService.GetCurrentUserInfoAsync(userEmail);
        
        var roles = GetUserRoles();
        
        if (roles.Contains(AuthPolicies.TeacherPolicy))
            user.Role = UserRole.Teacher;
        if (roles.Contains(AuthPolicies.AdminPolicy))
            user.Role = UserRole.Admin;
        
        return Ok(user);
    }
    
    [HttpGet("users")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userAppService.GetUsersAsync();
        
        return Ok(users);
    }
}