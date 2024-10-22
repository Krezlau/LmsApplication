using LmsApplication.UserModule.Data.Entities;
using LmsApplication.UserModule.Data.Models;

namespace LmsApplication.UserModule.Data.Mapping;

public static class UserMappingService
{
    public static UserModel ToModel(this User user)
    {
        if (user.Email is null || user.Roles is null) 
            throw new ArgumentException("Invalid model.");
        
        return new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            Roles = user.Roles.Select(r => new RoleModel
            {
                Id = r.Id,
                Name = r.Name!
            }).ToList()
        };
    }
}