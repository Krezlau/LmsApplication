using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using LmsApplication.Core.ApplicationServices.Auth;
using LmsApplication.Core.Config.ConfigModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthAppService _authAppService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthAppService authAppService,
        ILogger<AuthController> logger)
    {
        _authAppService = authAppService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> LoginUserAsync()
    {
        Console.WriteLine("LoginUserAsync");
        _logger.LogInformation(Request.Headers["X-Tenant-Id"]);
        _logger.LogCritical("Tenant Info:");
        return Ok("lmaooo");
    }
}