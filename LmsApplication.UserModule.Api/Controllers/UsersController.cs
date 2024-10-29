using System.Security.Claims;
using LmsApplication.Core.Data.Config;
using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using LmsApplication.UserModule.Data.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.UserModule.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly UserDbContext _userDbContext;

    public UsersController(UserManager<User> userManager, UserDbContext userDbContext)
    {
        _userManager = userManager;
        _userDbContext = userDbContext;
    }
    
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            throw new ArgumentException("Invalid user.");
        }

        var user = await _userDbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null) 
            throw new KeyNotFoundException("Couldn't find user with the given id.");

        return Ok(user.ToModel());
    }

    [HttpGet("{userEmail}")]
    public async Task<IActionResult> GetUserByEmail(string userEmail)
    {
        var user = await _userDbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == userEmail);
        if (user is null)
            throw new KeyNotFoundException("Couldn't find user with the given email.");

        return Ok(user.ToModel());
    }
    
    [HttpGet("")]
    [Authorize(AuthPolicies.AdminPolicy)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userDbContext.Users
            .Include(x => x.Roles)
            .ToListAsync();
        
        return Ok(users.Select(x => x.ToModel()).OrderByDescending(x => x.Role));
    }
}