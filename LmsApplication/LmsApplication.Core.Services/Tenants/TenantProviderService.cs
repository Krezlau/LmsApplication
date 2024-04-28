using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.Core.Services.Tenants;

public interface ITenantProviderService
{
    string GetTenantId();
}

public class TenantProviderService : ITenantProviderService
{
    private const string TenantIdHeader = "X-Tenant-Id";
    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public TenantProviderService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string GetTenantId()
    {
        var tenantIdHeaderValue = _httpContextAccessor.HttpContext?.Request.Headers[TenantIdHeader];

        if (tenantIdHeaderValue is null)
            throw new Exception("TenantId header is missing");
        
        var tenantId = tenantIdHeaderValue.ToString();
        if (tenantId is null)
            throw new Exception("TenantId header is missing");
        
        Console.WriteLine(tenantId);
        
        var tenantList = _configuration.GetSection("Tenants").GetChildren().Select(x => x.Value).ToHashSet();
        // print tenantList
        foreach (var tenant in tenantList)
        {
            Console.WriteLine(tenant);
        }
        
        // if (!tenantList.Contains(tenantId))
            // throw new Exception("Invalid TenantId header value");
        
        return tenantId;
    }
    
}