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
    
    public required string ApiClientId { get; set; }
    
    public required string ApiClientSecret { get; set; }
    
    public required string ApiTenantId { get; set; }
    
    public required string AdminRoleId { get; set; }
    
    public required string TeacherRoleId { get; set; }
    
    public required string Scope { get; set; }
}