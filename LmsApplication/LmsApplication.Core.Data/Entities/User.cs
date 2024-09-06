using Microsoft.AspNetCore.Identity;

namespace LmsApplication.Core.Data.Entities;

public class User : IdentityUser, IAuditable
{
    public Audit Audit { get; set; } = new();
}