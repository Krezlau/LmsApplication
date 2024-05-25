using LmsApplication.Core.Data.Enums;
using LmsApplication.Core.Data.Models.Users;
using Microsoft.Graph.Models;

namespace LmsApplication.Core.Data.Mapping;

public static class UserMappingService
{
    public static UserModel ToModel(this User user, AppRoleAssignmentCollectionResponse? userGroups, string adminRoleId,
        string teacherRoleId)
    {
        return new UserModel
        {
            Id = user.Id ?? "",
            Email = user.UserPrincipalName ?? "",
            Name = user.GivenName ?? "",
            Surname = user.Surname ?? "",
            Photo = user.Photo?.Id ?? null,
            Role = userGroups is not null ? DetermineUserRole(userGroups, adminRoleId, teacherRoleId) : null,
        };
    }
    
    private static UserRole? DetermineUserRole(AppRoleAssignmentCollectionResponse userGroups, string adminRoleId,
        string teacherRoleId)
    {
        if (userGroups.Value?.Any(x => x.AppRoleId.ToString() == adminRoleId) ?? false)
            return UserRole.Admin;
        
        if (userGroups.Value?.Any(x => x.AppRoleId.ToString() == teacherRoleId) ?? false)
            return UserRole.Teacher;
        
        return UserRole.Student;
    }
}