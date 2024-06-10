using System.Diagnostics;
using LmsApplication.Core.Data.Tenants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;

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

        // options = new MicrosoftIdentityOptions();
        // Other tenant-specific options like options.Authority can be registered here.
        options.Authority = tenant.OpenIdConnectAuthority;
        options.ClientId = tenant.OpenIdConnectClientId;
        options.TenantId = tenant.OpenIdTenantId;
        options.ClaimsIssuer = tenant.OpenIdClaimsIssuer;
        options.SignUpSignInPolicyId = tenant.OpenIdSignUpSignInPolicyId;
        
        options.TokenValidationParameters.ValidIssuers = new[] { tenant.OpenIdConnectAuthority, "https://sts.windows.net/93ba8a8c-bd33-4b98-af36-dcc63dc2f84e/", "https://sts.windows.net/40dcee2a-c051-4a80-acba-953dac9a3942/" };
        // write every parameter in options to the log without using json serializer
        _logger.LogInformation($"Options: {options}");
        
        options.Scope.Clear();
        options.Scope.Add(tenant.Scope);
    }

    public void Configure(MicrosoftIdentityOptions options)
        => Debug.Fail("This infrastructure method shouldn't be called.");
}