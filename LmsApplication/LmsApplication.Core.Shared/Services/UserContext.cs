using System.Security.Claims;
using LmsApplication.Core.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace LmsApplication.Core.Shared.Services;

public interface IUserContext
{
    string GetUserId();
    
    UserRole GetUserRole();
}

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId ?? throw new UnauthorizedAccessException();
    }

    public UserRole GetUserRole()
    {
        var role = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role);
        var roles = new List<UserRole>();
        foreach (var claim in role ?? throw new UnauthorizedAccessException())
        {
            if (Enum.TryParse<UserRole>(claim.Value, out var userRole)) roles.Add(userRole);
        }

        return roles.OrderDescending().FirstOrDefault();
    }
}