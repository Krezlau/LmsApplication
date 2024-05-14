using System.Collections.Concurrent;
using LmsApplication.Core.Data.Tenants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace LmsApplication.Api.AuthService.Infrastructure;

public class OpenIdConnectOptionsProvider : IOptionsMonitor<JwtBearerOptions>
{
    private readonly ConcurrentDictionary<(string name, string tenant), Lazy<JwtBearerOptions>> _cache;
    private readonly IOptionsFactory<JwtBearerOptions> _optionsFactory;
    private readonly ITenantProviderService _tenantProvider;
    private readonly ILogger<OpenIdConnectOptionsProvider> _logger;

    public OpenIdConnectOptionsProvider(
        IOptionsFactory<JwtBearerOptions> optionsFactory,
        ITenantProviderService tenantProvider,
        ILogger<OpenIdConnectOptionsProvider> logger)
    {
        _cache = new ConcurrentDictionary<(string, string), Lazy<JwtBearerOptions>>();
        _optionsFactory = optionsFactory;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public JwtBearerOptions CurrentValue => Get(Options.DefaultName);

    public JwtBearerOptions Get(string name)
    {
        var tenant = _tenantProvider.GetTenantId();
        _logger.LogInformation($"Getting JwtBearerOptions for tenant '{tenant}'.");

        Lazy<JwtBearerOptions> Create() => new Lazy<JwtBearerOptions>(() => _optionsFactory.Create(name));
        return _cache.GetOrAdd((name, tenant), _ => Create()).Value;
    }

    public IDisposable OnChange(Action<JwtBearerOptions, string> listener) => null;
}