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

    public AuthController(IAuthAppService authAppService)
    {
        _authAppService = authAppService;
    }

    [HttpGet]
    [Authorize]
    public async Task<string> LoginUserAsync([FromBody] LoginRequestDto model)
    {
        return await _authAppService.LoginUserAsync(model);
    }
}