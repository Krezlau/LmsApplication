using LmsApplication.Core.Data.Dtos;
using LmsApplication.Core.Services.Users;

namespace LmsApplication.Core.ApplicationServices.Auth;

public interface IAuthAppService
{
    Task<string> LoginUserAsync(LoginRequestDto model);
}

public class AuthAppService : IAuthAppService
{
    private readonly IUserService _userService;

    public AuthAppService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<string> LoginUserAsync(LoginRequestDto model)
    {
        return "lmao";
    }
}