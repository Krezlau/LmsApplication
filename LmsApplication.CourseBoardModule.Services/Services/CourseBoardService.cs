using LmsApplication.Core.Shared.Providers;

namespace LmsApplication.CourseBoardModule.Services.Services;

public abstract class CourseBoardService
{
    protected readonly ICourseEditionProvider CourseEditionProvider;
    protected readonly IUserProvider UserProvider;

    protected CourseBoardService(ICourseEditionProvider courseEditionProvider, IUserProvider userProvider)
    {
        CourseEditionProvider = courseEditionProvider;
        UserProvider = userProvider;
    }
    
    protected async Task ValidateUserAccessToEditionAsync(Guid editionId, string userId)
    {
        var isAdmin = UserProvider.IsUserAdminAsync(userId);
        var isRegistered = CourseEditionProvider.IsUserRegisteredToCourseEditionAsync(editionId, userId);
        
        if (!await isRegistered && !await isAdmin)
            throw new UnauthorizedAccessException("User is not registered to course edition.");
    }
}