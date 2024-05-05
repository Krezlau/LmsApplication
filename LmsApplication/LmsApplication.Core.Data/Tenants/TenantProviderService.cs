using LmsApplication.Core.Config.ConfigModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LmsApplication.Core.Data.Tenants;

public interface ITenantProviderService
{
    string GetTenantId();
}

public class TenantProviderService : ITenantProviderService
{
    private const string TenantIdHeader = "X-Tenant-Id";
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantsModel _tenantsModel;

    public TenantProviderService(IHttpContextAccessor httpContextAccessor, IOptions<TenantsModel> tenantsModel)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantsModel = tenantsModel.Value;
    }

    public string GetTenantId()
    {
        Console.WriteLine(_tenantsModel);
        Console.WriteLine(_tenantsModel.Tenants);
        Console.WriteLine(_tenantsModel.Tenants.Length);
        foreach (var tenant in _tenantsModel.Tenants)
        {
            Console.WriteLine(tenant);
        }
        
        var tenantIdHeaderValue = _httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeader];

        if (tenantIdHeaderValue is null)
            throw new ArgumentException("TenantId header is missing");
        
        var tenantId = tenantIdHeaderValue.ToString();
        if (tenantId is null)
            throw new ArgumentException("TenantId header is missing");
        
        if (!_tenantsModel.Tenants.Contains(tenantId))
            throw new ArgumentException("Invalid TenantId header value");
        
        Console.WriteLine(tenantId);
        return tenantId;
    }
    
}