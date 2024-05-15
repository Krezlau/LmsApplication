using System.Diagnostics;
using LmsApplication.Core.Data.Tenants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace LmsApplication.Api.AuthService.Infrastructure;

public class JwtBearerOptionsInitializer : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly ITenantProviderService _tenantProvider;
    private readonly ILogger<JwtBearerOptionsInitializer> _logger;

    public JwtBearerOptionsInitializer(
        ITenantProviderService tenantProvider,
        ILogger<JwtBearerOptionsInitializer> logger)
    {
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        if (!string.Equals(name, JwtBearerDefaults.AuthenticationScheme, StringComparison.Ordinal))
            return;
        
        var tenant = _tenantProvider.GetTenantInfo();

        options.Audience = tenant.OpenIdConnectAudience;
    }

    public void Configure(JwtBearerOptions options)
        => Debug.Fail("This infrastructure method shouldn't be called.");
}
