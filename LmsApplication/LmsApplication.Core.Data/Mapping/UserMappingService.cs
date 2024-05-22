using LmsApplication.Core.Data.Enums;
using LmsApplication.Core.Data.Models.Users;
using Microsoft.Graph.Models;

namespace LmsApplication.Core.Data.Mapping;

public static class UserMappingService
{
    public static UserModel ToModel(this User user, List<Group>? userGroups)
    {
        return new UserModel
        {
            Id = user.Id ?? "",
            Email = user.DisplayName ?? "",
            Name = user.GivenName ?? "",
            Surname = user.Surname ?? "",
            Photo = user.Photo?.Id ?? null,
            Role = userGroups is not null ? DetermineUserRole(userGroups) : null,
        };
    }
    
    private static UserRole? DetermineUserRole(List<Group> userGroups)
    {
        if (userGroups.Any(x => x.DisplayName == "Admin"))
            return UserRole.Admin;
        
        if (userGroups.Any(x => x.DisplayName == "Teacher"))
            return UserRole.Teacher;
        
        return UserRole.Student;
    }
}