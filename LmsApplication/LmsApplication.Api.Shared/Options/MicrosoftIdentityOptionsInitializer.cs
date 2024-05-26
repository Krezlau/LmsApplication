using System.Diagnostics;
using LmsApplication.Core.Data.Tenants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace LmsApplication.Api.Shared.Options;

public class MicrosoftIdentityOptionsInitializer : IConfigureNamedOptions<MicrosoftIdentityOptions>
{
    private readonly ITenantProviderService _tenantProvider;
    private readonly ILogger<MicrosoftIdentityOptionsInitializer> _logger;

    public MicrosoftIdentityOptionsInitializer(
        ITenantProviderService tenantProvider,
        ILogger<MicrosoftIdentityOptionsInitializer> logger)
    {
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public void Configure(string name, MicrosoftIdentityOptions options)
    {
        var tenant = _tenantProvider.GetTenantInfo();
        _logger.LogInformation($"Configuring MicrosoftIdentityOptions for tenant '{tenant}'.");

        // Other tenant-specific options like options.Authority can be registered here.
        options.Authority = tenant.OpenIdConnectAuthority;
        options.ClientId = tenant.OpenIdConnectClientId;
        options.TenantId = tenant.OpenIdTenantId;
        options.ClaimsIssuer = tenant.OpenIdClaimsIssuer;
        options.SignUpSignInPolicyId = tenant.OpenIdSignUpSignInPolicyId;
        
        options.Scope.Clear();
        options.Scope.Add(tenant.Scope);
    }

    public void Configure(MicrosoftIdentityOptions options)
        => Debug.Fail("This infrastructure method shouldn't be called.");
}