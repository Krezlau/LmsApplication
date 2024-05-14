
using Finbuckle.MultiTenant;

namespace LmsApplication.Core.Config.ConfigModels;

public class TenantsModel
{
    public static string Key = "App:Tenants";
    
    public required TenantInfo[] Tenants { get; set; }
}

public class AppTenantsModel
{
    public static string Key = "App:Tenants";
    
    public required AppTenantInfo[] Tenants { get; set; }
}