using LmsApplication.Core.Data.Mapping;
using LmsApplication.Core.Data.Models.Users;
using LmsApplication.Core.Data.Tenants;
using LmsApplication.Core.Services.Graph;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;

namespace LmsApplication.Core.ApplicationServices.Users;

public interface IUserAppService
{
    Task<UserModel> GetUserInfoAsync(string userEmail);
    
    Task<UserModel> GetCurrentUserInfoAsync(string userEmail);
    
    Task<List<UserModel>> GetUsersAsync();
}

public class UserAppService : IUserAppService
{
    private readonly IGraphService _graphService;
    private readonly ILogger<UserAppService> _logger;
    private readonly ITenantProviderService _tenantProviderService;

    public UserAppService(IGraphService graphService,
        ILogger<UserAppService> logger,
        ITenantProviderService tenantProviderService)
    {
        _graphService = graphService;
        _logger = logger;
        _tenantProviderService = tenantProviderService;
    }

    public async Task<UserModel> GetUserInfoAsync(string userEmail)
    {
        var user = await _graphService.GetUserAsync(userEmail);
        var groups = await _graphService.GetUserRolesAsync(userEmail);
        
        var tenantInfo = _tenantProviderService.GetTenantInfo();
        return user?.ToModel(groups, tenantInfo.AdminRoleId, tenantInfo.TeacherRoleId)
               ?? throw new KeyNotFoundException($"{nameof(UserModel)} not found.");
    }

    public async Task<UserModel> GetCurrentUserInfoAsync(string userEmail)
    {
        var user = await _graphService.GetCurrentUserInfoAsync(userEmail);
        return user.ToModel(null, null, null);
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        var users = await _graphService.GetUsersAsync();
        var groups = new List<AppRoleAssignmentCollectionResponse>();
        foreach (var user in users)
        {
            groups.Add(await _graphService.GetUserRolesAsync(user.UserPrincipalName ?? ""));
        }
        
        var tenantInfo = _tenantProviderService.GetTenantInfo();
        return users.Select((user, index) => user.ToModel(groups[index], tenantInfo.AdminRoleId, tenantInfo.TeacherRoleId)).ToList();
    }
}