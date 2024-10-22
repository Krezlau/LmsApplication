namespace LmsApplication.UserModule.Data.Models;

public class UserModel
{
    public string Id { get; set; } = string.Empty;
        
    public string Email { get; set; } = string.Empty;
    
    public string? Name { get; set; }
    
    public string? Surname { get; set; }
    
    public string? Photo { get; set; }
    
    public List<RoleModel> Roles { get; set; } = new ();
}