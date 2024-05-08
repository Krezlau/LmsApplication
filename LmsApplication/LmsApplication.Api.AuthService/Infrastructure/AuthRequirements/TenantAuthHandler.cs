using System.Security.Claims;
using LmsApplication.Core.Data.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Api.AuthService.Infrastructure.AuthRequirements;

public class TenantAuthHandler : AuthorizationHandler<TenantAuthRequirement>
{
    private readonly AuthDbContext _authDbContext;

    public TenantAuthHandler(AuthDbContext authDbContext)
    {
        _authDbContext = authDbContext;
    }

    // TODO Find a proper way to do it
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantAuthRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"UserId: {userId}");
        if (userId is null)
        {
            context.Fail();
            return;
        }
        
        var user = await _authDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        Console.WriteLine($"User: {user}");
        if (user is null) 
        {
            context.Fail();
            return;
        }
        
        context.Succeed(requirement);
    }
}