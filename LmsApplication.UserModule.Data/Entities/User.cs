using Microsoft.AspNetCore.Identity;

namespace LmsApplication.UserModule.Data.Entities;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;

    public string? Bio { get; set; }
    
    public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = default!;
    
    public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; } = default!;

    public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; } = default!;

    public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = default!;
    
    public virtual ICollection<IdentityRole> Roles { get; set; } = default!;
}