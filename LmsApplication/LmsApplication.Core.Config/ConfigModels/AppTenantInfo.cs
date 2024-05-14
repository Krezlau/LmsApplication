using Finbuckle.MultiTenant;

namespace LmsApplication.Core.Config.ConfigModels;

public class AppTenantInfo : TenantInfo
{
    public string? Id { get; set; }
    public string? Identifier { get; set; }
    public string? Name { get; set; }
    public string? OpenIdConnectAuthority { get; set; }
    public string? OpenIdConnectClientId { get; set; }
    public string? OpenIdConnectClientSecret { get; set; }
}