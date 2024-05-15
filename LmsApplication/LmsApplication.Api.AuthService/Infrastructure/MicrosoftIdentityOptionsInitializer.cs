using System.Diagnostics;
using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Core.Data.Tenants;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace LmsApplication.Api.AuthService.Infrastructure;

public class MicrosoftIdentityOptionsInitializer : IConfigureNamedOptions<MicrosoftIdentityOptions>
{
    private readonly ITenantProviderService _tenantProvider;
    private readonly AppTenantsModel _tenants; 
    private readonly ILogger<MicrosoftIdentityOptionsInitializer> _logger;

    public MicrosoftIdentityOptionsInitializer(
        ITenantProviderService tenantProvider,
        IOptions<AppTenantsModel> tenants,
        ILogger<MicrosoftIdentityOptionsInitializer> logger)
    {
        _tenantProvider = tenantProvider;
        _logger = logger;
        _tenants = tenants.Value;
    }

    public void Configure(string name, MicrosoftIdentityOptions options)
    {
        var tenant = _tenantProvider.GetTenantId();
        _logger.LogInformation($"Configuring MicrosoftIdentityOptions for tenant '{tenant}'.");
        
        // print all tenants
        foreach (var t in _tenants.Tenants)
        {
            _logger.LogInformation($"Tenant: {t.Identifier}");
        }
        
        var tenantInfo = _tenants.Tenants.FirstOrDefault(t => t.Identifier == tenant);
        _logger.LogInformation($"Tenant Info: {tenantInfo.OpenIdConnectAuthority}");

        // Other tenant-specific options like options.Authority can be registered here.
        options.Authority = tenantInfo.OpenIdConnectAuthority;
        options.ClientId = tenantInfo.OpenIdConnectClientId;
        options.TokenValidationParameters.ValidAudience = "api://lms-api";
        // options.ClientSecret = tenantInfo.OpenIdConnectClientSecret;
        // options.Instance = "https://login.microsoftonline.com/";
        options.TenantId = "common";
        options.ClaimsIssuer = $"https://login.microsoftonline.com/40dcee2a-c051-4a80-acba-953dac9a3942/v2.0";
        options.Scope.Clear();
        options.Scope.Add("tasks.read");
        options.SignUpSignInPolicyId = "b2x_susi";
        // options.TokenValidationParameters.ValidateIssuerSigningKey = false;
    }

    public void Configure(MicrosoftIdentityOptions options)
        => Debug.Fail("This infrastructure method shouldn't be called.");
}