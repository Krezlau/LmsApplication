using LmsApplication.Core.Config.ConfigModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LmsApplication.Core.Data.Tenants;

public interface ITenantProviderService
{
    string GetTenantId();
    
    AppTenantInfo GetTenantInfo();
}

public class TenantProviderService : ITenantProviderService
{
    private const string TenantIdHeader = "X-Tenant-Id";
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppTenantsModel _tenantsModel;

    public TenantProviderService(IHttpContextAccessor httpContextAccessor, IOptions<AppTenantsModel> tenantsModel)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantsModel = tenantsModel.Value;
    }

    public string GetTenantId()
    {
        var tenantIdHeaderValue = _httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeader];

        if (tenantIdHeaderValue is null)
            throw new ArgumentException("TenantId header is missing");
        
        var tenantId = tenantIdHeaderValue.ToString();
        if (tenantId is null)
            throw new ArgumentException("TenantId header is missing");
        
        if (!_tenantsModel.Tenants.Select(x => x.Identifier).Contains(tenantId))
            throw new ArgumentException("Invalid TenantId header value");
        
        return tenantId;
    }
    
    public AppTenantInfo GetTenantInfo()
    {
        var tenantId = GetTenantId();
        return _tenantsModel.Tenants.FirstOrDefault(x => x.Identifier == tenantId)!;
    }
    
}