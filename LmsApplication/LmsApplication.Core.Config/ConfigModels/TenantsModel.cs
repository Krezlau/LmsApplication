namespace LmsApplication.Core.Config.ConfigModels;

public class AppTenantsModel
{
    public static string Key = "App:Tenants";
    
    public required AppTenantInfo[] Tenants { get; set; }
}