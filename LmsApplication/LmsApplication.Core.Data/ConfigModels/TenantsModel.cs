namespace LmsApplication.Core.Data.ConfigModels;

public class TenantsModel
{
    public static string Key = "App:Tenants";
    
    public required string[] Tenants { get; set; }
}