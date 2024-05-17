using System.Security.Claims;
using LmsApplication.Core.ApplicationServices.Users;
using LmsApplication.Core.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserAppService _userAppService;

    public AuthController(ILogger<AuthController> logger, IUserAppService userAppService)
    {
        _logger = logger;
        _userAppService = userAppService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userEmail = GetUserEmail();
        var user = await _userAppService.GetCurrentUserInfoAsync(userEmail);
        
        return Ok(user);
    }
    
    [HttpGet("users")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userAppService.GetUsersAsync();
        
        return Ok(users);
    }
    
    private string GetUserEmail()
    {
        var email = User.FindFirstValue(ClaimTypes.Name);
        if (email is null) 
            throw new KeyNotFoundException("User email not found.");
        return email;
    }
}