using LmsApplication.Core.Shared.Enums;

namespace LmsApplication.UserModule.Data.Models;

public class UserModel
{
    public string Id { get; set; } = string.Empty;
        
    public string Email { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Surname { get; set; } = string.Empty;
    
    public string? Photo { get; set; }

    public UserRole Role { get; set; } = UserRole.Student;
}