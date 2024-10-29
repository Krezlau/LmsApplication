namespace LmsApplication.UserModule.Data.Models;

public class RegisterRequestModel
{
    public required string Email { get; init; }
    
    public required string Password { get; init; }
    
    public required string Name { get; init; }
    
    public required string Surname { get; init; }
}