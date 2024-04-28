using System.Net;
using LmsApplication.Core.Data.Database;
using LmsApplication.Core.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Api.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _authDbContext;

    public AuthController(AuthDbContext authDbContext)
    {
        _authDbContext = authDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _authDbContext.Users.Add(new User());
        await _authDbContext.SaveChangesAsync();
        return Ok(await _authDbContext.Users.ToListAsync());
    }
}