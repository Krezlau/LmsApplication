using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace LmsApplication.Api.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly GraphServiceClient _graphServiceClient;

    public AuthController(ILogger<AuthController> logger, GraphServiceClient graphServiceClient)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> LoginUserAsync()
    {
        var response = await _graphServiceClient.Me.GetAsync();
        return Ok(response);
    }
}