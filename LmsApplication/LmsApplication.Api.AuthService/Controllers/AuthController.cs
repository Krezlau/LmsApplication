using System.Security.Claims;
using LmsApplication.Core.Services.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IMicrosoftGraphServiceProvider _graphServiceClientProvider;

    public AuthController(ILogger<AuthController> logger, IMicrosoftGraphServiceProvider graphServiceClientProvider)
    {
        _logger = logger;
        _graphServiceClientProvider = graphServiceClientProvider;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var graphClient = _graphServiceClientProvider.GetGraphServiceClient();
        var currentUser = await graphClient.Users[GetUserEmail()].GetAsync();
        if (currentUser is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        return Ok(currentUser);
    }
    
    [HttpGet("teacher")]
    [Authorize("Teacher")]
    public async Task<IActionResult> GetTeacher()
    {
        var graphClient = _graphServiceClientProvider.GetGraphServiceClient();
        var teacher = await graphClient.Users[GetUserEmail()].GetAsync();
        if (teacher is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        return Ok(teacher);
    }
    
    [HttpGet("student")] 
    [Authorize]
    public async Task<IActionResult> GetStudent()
    {
        var graphClient = _graphServiceClientProvider.GetGraphServiceClient();
        var student = await graphClient.Users[GetUserEmail()].GetAsync();
        if (student is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        return Ok(student);
    }
    
    [HttpGet("admin")]
    [Authorize("Admin")]
    public async Task<IActionResult> GetAdmin()
    {
        var graphClient = _graphServiceClientProvider.GetGraphServiceClient();
        var admin = await graphClient.Users[GetUserEmail()].GetAsync();
        if (admin is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        return Ok(admin);
    }
    
    [HttpGet("anonymous")]
    [AllowAnonymous]
    public IActionResult GetAnonymous()
    {
        // print roles
        User.Claims.Where(claim => claim.Type == ClaimTypes.Role).ToList().ForEach(claim => _logger.LogInformation($"{claim.Type}: {claim.Value}"));
        
        User.Claims.ToList().ForEach(claim => _logger.LogInformation($"{claim.Type}: {claim.Value}"));
        return Ok("Hello, anonymous!");
    }
    
    private string GetUserEmail()
    {
        var email = User.FindFirstValue(ClaimTypes.Name);
        return email;
    }
}