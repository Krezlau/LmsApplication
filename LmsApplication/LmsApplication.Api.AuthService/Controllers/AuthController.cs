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
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantInfo;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthAppService authAppService, IMultiTenantContextAccessor<AppTenantInfo> tenantInfo, ILogger<AuthController> logger)
    {
        _authAppService = authAppService;
        _tenantInfo = tenantInfo;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> LoginUserAsync()
    {
        foreach (var tenant in _tenantInfo.MultiTenantContext.StoreInfo.Store.GetAllAsync().Result)
        {
            _logger.LogInformation($"Tenant: {tenant.Identifier}");
        }
        _logger.LogInformation(Request.Headers["X-Tenant-Id"]);
        return Ok(_tenantInfo.MultiTenantContext.TenantInfo);
    }
}