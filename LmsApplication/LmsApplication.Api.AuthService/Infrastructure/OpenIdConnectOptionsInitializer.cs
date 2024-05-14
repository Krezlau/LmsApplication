using System.Diagnostics;
using LmsApplication.Core.Config.ConfigModels;
using LmsApplication.Core.Data.Tenants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace LmsApplication.Api.AuthService.Infrastructure;

public class OpenIdConnectOptionsInitializer : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly ITenantProviderService _tenantProvider;
    private readonly AppTenantsModel _tenants; 
    private readonly ILogger<OpenIdConnectOptionsInitializer> _logger;

    public OpenIdConnectOptionsInitializer(
        ITenantProviderService tenantProvider,
        IOptions<AppTenantsModel> tenants,
        ILogger<OpenIdConnectOptionsInitializer> logger)
    {
        _tenantProvider = tenantProvider;
        _logger = logger;
        _tenants = tenants.Value;
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        if (!string.Equals(name, JwtBearerDefaults.AuthenticationScheme, StringComparison.Ordinal))
        {
            return;
        }

        var tenant = _tenantProvider.GetTenantId();
        _logger.LogInformation($"Configuring JwtBearerOptions for tenant '{tenant}'.");
        
        // print all tenants
        foreach (var t in _tenants.Tenants)
        {
            _logger.LogInformation($"Tenant: {t.Identifier}");
        }
        
        var tenantInfo = _tenants.Tenants.FirstOrDefault(t => t.Identifier == tenant);
        _logger.LogInformation($"Tenant Info: {tenantInfo.OpenIdConnectAuthority}");

        // Other tenant-specific options like options.Authority can be registered here.
        options.Authority = tenantInfo.OpenIdConnectAuthority;
        options.Audience = tenantInfo.OpenIdConnectClientId;
        options.TokenValidationParameters.ValidateIssuerSigningKey = false;
        options.TokenValidationParameters.ValidateIssuer = false;
        options.TokenValidationParameters.ValidateActor = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidateLifetime = false;
    }

    public void Configure(JwtBearerOptions options)
        => Debug.Fail("This infrastructure method shouldn't be called.");
}
