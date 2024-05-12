using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using LmsApplication.Core.ApplicationServices.Auth;
using LmsApplication.Core.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LmsApplication.Api.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthAppService _authAppService;
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantInfo;

    public AuthController(IAuthAppService authAppService, IMultiTenantContextAccessor<TenantInfo> tenantInfo)
    {
        _authAppService = authAppService;
        _tenantInfo = tenantInfo;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> LoginUserAsync()
    {
        return Ok(_tenantInfo.MultiTenantContext.TenantInfo);
    }
}