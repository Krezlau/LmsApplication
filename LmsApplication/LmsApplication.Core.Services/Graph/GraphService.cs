using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Newtonsoft.Json;

namespace LmsApplication.Core.Services.Graph;

public interface IGraphService
{
    Task<User> GetCurrentUserInfoAsync(string userEmail);
    
    Task<List<Group>> GetUserGroupsAsync(string userEmail);
    
    Task<List<User>> GetUsersAsync();
}

public class GraphService : IGraphService
{
    private readonly IMicrosoftGraphServiceProvider _graphServiceProvider;
    private readonly ILogger<GraphService> _logger;

    public GraphService(IMicrosoftGraphServiceProvider graphServiceProvider, ILogger<GraphService> logger)
    {
        _graphServiceProvider = graphServiceProvider;
        _logger = logger;
    }

    public async Task<User> GetCurrentUserInfoAsync(string userEmail)
    {
        var graphClient = _graphServiceProvider.GetGraphServiceClient();
        
        var currentUser = await graphClient.Users[userEmail].GetAsync();
        if (currentUser is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        
        _logger.LogInformation("EXECUTED: {MethodName}, return: {object}", "GetCurrentUserInfoAsync",
            JsonConvert.SerializeObject(currentUser));
        return currentUser;
    }

    public async Task<List<Group>> GetUserGroupsAsync(string userEmail)
    {
        var graphClient = _graphServiceProvider.GetGraphServiceClient();
        
        var userGroups = await graphClient.Users[userEmail].MemberOf.GetAsync();
        if (userGroups?.Value is null || userGroups.Value.Count == 0)
            return new List<Group>();
        
        _logger.LogInformation("EXECUTED: {MethodName}, return: {object}", "GetUserGroupsAsync",
            JsonConvert.SerializeObject(userGroups));
        
        List<Group> groups = userGroups.Value
            .Select(x => x as Group)
            .Where(x => x is not null)
            .ToList()!;
        
        return groups;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var graphClient = _graphServiceProvider.GetGraphServiceClient();
        
        var users = await graphClient.Users.GetAsync();
        if (users?.Value is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        
        _logger.LogInformation("EXECUTED: {MethodName}, return: {object}", "GetUsersAsync",
            JsonConvert.SerializeObject(users));
        return users.Value;
    }
}