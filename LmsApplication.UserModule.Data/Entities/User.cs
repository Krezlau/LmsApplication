using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LmsApplication.UserModule.Data.Entities;

public class User : IdentityUser
{
    [PersonalData]
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [PersonalData]
    [Required]
    public string Surname { get; set; } = string.Empty;
    
    public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = default!;
    
    public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; } = default!;

    public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; } = default!;

    public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = default!;
    
    public virtual ICollection<IdentityRole> Roles { get; set; } = default!;
}