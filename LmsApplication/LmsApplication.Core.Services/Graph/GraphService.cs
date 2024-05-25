using LmsApplication.Core.Data.Tenants;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Newtonsoft.Json;

namespace LmsApplication.Core.Services.Graph;

public interface IGraphService
{
    Task<User?> GetUserAsync(string userEmail);
    
    Task<User> GetCurrentUserInfoAsync(string userEmail);
    
    Task<List<User>> GetUsersAsync();
    
    Task<AppRoleAssignmentCollectionResponse> GetUserRolesAsync(string userEmail);
}

public class GraphService : IGraphService
{
    private readonly IMicrosoftGraphServiceProvider _graphServiceProvider;
    private readonly ILogger<GraphService> _logger;
    private readonly ITenantProviderService _tenantProviderService;

    public GraphService(IMicrosoftGraphServiceProvider graphServiceProvider,
        ILogger<GraphService> logger,
        ITenantProviderService tenantProviderService)
    {
        _graphServiceProvider = graphServiceProvider;
        _logger = logger;
        _tenantProviderService = tenantProviderService;
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
    
    public async Task<User?> GetUserAsync(string userEmail)
    {
        var graphClient = _graphServiceProvider.GetGraphServiceClient();
        
        var user = await graphClient.Users[userEmail].GetAsync();
        if (user is null) 
            throw new KeyNotFoundException($"{nameof(User)} not found.");
        
        _logger.LogInformation("EXECUTED: {MethodName}, return: {object}", "GetUserAsync",
            JsonConvert.SerializeObject(user));
        return user;
    }
    
    public async Task<AppRoleAssignmentCollectionResponse> GetUserRolesAsync(string userEmail)
    {
        var graphClient = _graphServiceProvider.GetGraphServiceClient();
        
        var userRoles = await graphClient.Users[userEmail].AppRoleAssignments.GetAsync(config =>
        {
            // config.QueryParameters.Filter = $"resourceId eq {_tenantProviderService.GetTenantInfo().ApiClientId}";
        });
        if (userRoles is null) 
            throw new KeyNotFoundException($"{nameof(AppRoleAssignmentCollectionResponse)} not found.");
        
        _logger.LogInformation("EXECUTED: {MethodName}, return: {object}", "GetUserRolesAsync",
            JsonConvert.SerializeObject(userRoles));
        return userRoles;
    }
}