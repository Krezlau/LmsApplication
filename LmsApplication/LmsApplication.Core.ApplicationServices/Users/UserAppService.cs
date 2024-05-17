using LmsApplication.Core.Services.Graph;
using Microsoft.Graph.Models;

namespace LmsApplication.Core.ApplicationServices.Users;

public interface IUserAppService
{
    Task<User> GetCurrentUserInfoAsync(string userEmail);
    
    Task<List<User>> GetUsersAsync();
}

public class UserAppService : IUserAppService
{
    private readonly IGraphService _graphService;

    public UserAppService(IGraphService graphService)
    {
        _graphService = graphService;
    }

    public async Task<User> GetCurrentUserInfoAsync(string userEmail)
    {
        return await _graphService.GetCurrentUserInfoAsync(userEmail);
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _graphService.GetUsersAsync();
    }
}