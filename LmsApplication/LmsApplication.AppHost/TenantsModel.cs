namespace LmsApplication.AppHost;

// not the best 
public class TenantsModel
{
    public static string Key = "App:Tenants";
    
    public required string[] Tenants { get; set; }
}