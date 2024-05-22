using LmsApplication.Core.Data.Mapping;
using LmsApplication.Core.Data.Models.Users;
using LmsApplication.Core.Services.Graph;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Core.ApplicationServices.Users;

public interface IUserAppService
{
    Task<UserModel> GetCurrentUserInfoAsync(string userEmail);
    
    Task<List<UserModel>> GetUsersAsync();
}

public class UserAppService : IUserAppService
{
    private readonly IGraphService _graphService;
    private readonly ILogger<UserAppService> _logger;

    public UserAppService(IGraphService graphService, ILogger<UserAppService> logger)
    {
        _graphService = graphService;
        _logger = logger;
    }

    public async Task<UserModel> GetCurrentUserInfoAsync(string userEmail)
    {
        var user = await _graphService.GetCurrentUserInfoAsync(userEmail);
        var userGroups = await _graphService.GetUserGroupsAsync(userEmail);
        foreach(var usergroup in userGroups)
        {
            _logger.LogInformation(usergroup.DisplayName);
        }
        return user.ToModel(userGroups);
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        return (await _graphService.GetUsersAsync()).Select(x => x.ToModel(null)).ToList();
    }
}