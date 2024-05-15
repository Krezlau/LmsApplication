namespace LmsApplication.Core.Config.ConfigModels;

public class AppTenantInfo
{
    public required string Id { get; set; }
    
    public required string Identifier { get; set; }
    
    public required string Name { get; set; }
    
    public required string OpenIdConnectAuthority { get; set; }
    
    public required string OpenIdConnectClientId { get; set; }
    
    public required string OpenIdConnectAudience { get; set; }

    public string? OpenIdTenantId { get; set; } = "common";
    
    public required string OpenIdClaimsIssuer { get; set; }
    
    public required string OpenIdSignUpSignInPolicyId { get; set; }
}