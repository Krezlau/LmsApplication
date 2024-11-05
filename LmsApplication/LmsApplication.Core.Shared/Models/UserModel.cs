using LmsApplication.Core.Shared.Enums;

namespace LmsApplication.Core.Shared.Models;

public class UserExchangeModel
{
    public required string Id { get; set; }
    
    public required string Email { get; set; }
    
    public required string Name { get; set; }
    
    public required string Surname { get; set; }
    
    public required UserRole Role { get; set; }
}