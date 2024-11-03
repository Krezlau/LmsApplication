using LmsApplication.Core.Shared.Enums;
using LmsApplication.UserModule.Data.Entities;
using LmsApplication.UserModule.Data.Models;

namespace LmsApplication.UserModule.Data.Mapping;

public static class UserMappingService
{
    public static UserModel ToModel(this User user)
    {
        if (user.Email is null || user.Roles is null) 
            throw new ArgumentException("Invalid model.");

        var userRole = UserRole.Student;
        if (user.Roles.Any(x => x.Name?.ToUpper() == "TEACHER")) userRole = UserRole.Teacher;
        if (user.Roles.Any(x => x.Name?.ToUpper() == "ADMIN")) userRole = UserRole.Admin;
        
        return new UserModel
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Username = user.UserName!,
            Role = userRole,
        };
    }
}