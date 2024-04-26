using Microsoft.AspNetCore.Http;

namespace LmsApplication.Core.Services.Tenants;

public interface ITenantProviderService
{
    Guid GetTenantId();
}

public class TenantProviderService : ITenantProviderService
{
    private const string TenantIdHeader = "X-Tenant-Id";
    
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProviderService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetTenantId()
    {
        var tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeader];
        
        if (!Guid.TryParse(tenantIdHeader, out var tenantId))
        {
            throw new ApplicationException("Tenant Id is not valid");
        }
        
        // TODO add validation with tenant list
        
        return tenantId;
    }
    
}