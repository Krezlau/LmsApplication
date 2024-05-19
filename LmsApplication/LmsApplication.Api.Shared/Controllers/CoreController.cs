using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Api.Shared.Controllers;

[ApiController]
[Authorize]
public class CoreController : ControllerBase
{
    private readonly ILogger<CoreController> _logger;

    public CoreController(ILogger<CoreController> logger)
    {
        _logger = logger;
    }

    protected string GetUserEmail()
    {
        var email = User.FindFirstValue(ClaimTypes.Name);
        if (email is null) 
            throw new KeyNotFoundException("User email not found.");
        return email;
    }
}