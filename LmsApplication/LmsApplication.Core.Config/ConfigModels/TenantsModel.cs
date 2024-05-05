namespace LmsApplication.Core.Config.ConfigModels;

public class TenantsModel
{
    public static string Key = "App:Tenants";
    
    public required string[] Tenants { get; set; }
}