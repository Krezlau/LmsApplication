using Azure.Identity;
using LmsApplication.Core.Data.Tenants;
using Microsoft.Graph;

namespace LmsApplication.Core.Services.Graph;

public interface IMicrosoftGraphServiceProvider
{
    GraphServiceClient GetGraphServiceClient();
}

public class MicrosoftGraphServiceProvider : IMicrosoftGraphServiceProvider
{
    private readonly ITenantProviderService _tenantProvider;

    public MicrosoftGraphServiceProvider(ITenantProviderService tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public GraphServiceClient GetGraphServiceClient()
    {
        var tenant = _tenantProvider.GetTenantInfo();
        
        var scopes = new[] { "https://graph.microsoft.com/.default" };
        
        // Values from app registration
        var clientId = tenant.ApiClientId;
        var tenantId = tenant.ApiTenantId;
        var clientSecret = tenant.ApiClientSecret;
        
        // using Azure.Identity;
        var options = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };
        
        // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
        var clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret, options);
        
        var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

        return graphClient;
    }
}