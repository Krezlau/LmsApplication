using System.Collections.Concurrent;
using LmsApplication.Core.Data.Tenants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace LmsApplication.Api.Shared.Options;

public class MicrosoftIdentityOptionsProvider : IOptionsMonitor<MicrosoftIdentityOptions>
{
        private readonly ConcurrentDictionary<(string name, string tenant), Lazy<MicrosoftIdentityOptions>> _cache;
        private readonly IOptionsFactory<MicrosoftIdentityOptions> _optionsFactory;
        private readonly ITenantProviderService _tenantProvider;
        private readonly ILogger<MicrosoftIdentityOptionsProvider> _logger;
    
        public MicrosoftIdentityOptionsProvider(
            IOptionsFactory<MicrosoftIdentityOptions> optionsFactory,
            ITenantProviderService tenantProvider,
            ILogger<MicrosoftIdentityOptionsProvider> logger)
        {
            _cache = new ConcurrentDictionary<(string, string), Lazy<MicrosoftIdentityOptions>>();
            _optionsFactory = optionsFactory;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }
    
        public MicrosoftIdentityOptions CurrentValue => Get(Microsoft.Extensions.Options.Options.DefaultName);
    
        public MicrosoftIdentityOptions Get(string name)
        {
            var tenant = _tenantProvider.GetTenantId();
            _logger.LogInformation($"Getting JwtBearerOptions for tenant '{tenant}'.");
    
            Lazy<MicrosoftIdentityOptions> Create() => new Lazy<MicrosoftIdentityOptions>(() => _optionsFactory.Create(name));
            return _cache.GetOrAdd((name, tenant), _ => Create()).Value;
        }
    
        public IDisposable OnChange(Action<MicrosoftIdentityOptions, string> listener) => null;
}