
using Microsoft.Graph.Models;

namespace LmsApplication.Core.Services.Graph;

public interface IGraphService
{
    Task<User> GetCurrentUserInfoAsync(string userEmail);
    
    Task<List<User>> GetUsersAsync();
}

public class GraphService : IGraphService
{
    private readonly IMicrosoftGraphServiceProvider _graphServiceProvider;

    public GraphService(IMicrosoftGraphServiceProvider graphServiceProvider)
    {
        _graphServiceProvider = graphServiceProvider;
    }

    public async Task<User> GetCurrentUserInfoAsync(string userEmail)
    {
        var graphClient = _graphServiceProvider.GetGraphServiceClient();
        
        var currentUser = await graphClient.Users[userEmail].GetAsync();
        if (currentUser is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        
        return currentUser;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var graphClient = _graphServiceProvider.GetGraphServiceClient();
        
        var users = await graphClient.Users.GetAsync();
        if (users?.Value is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        
        return users.Value;
    }
}