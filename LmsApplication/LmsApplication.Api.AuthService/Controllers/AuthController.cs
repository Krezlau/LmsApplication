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
    
    private string GetUserEmail()
    {
        var email = User.FindFirstValue(ClaimTypes.Name);
        return email;
    }
}