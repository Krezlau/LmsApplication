using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Services.Providers;

namespace LmsApplication.CourseBoardModule.Services.Services;

public abstract class CourseBoardService
{
    protected readonly ICourseEditionProvider CourseEditionProvider;
    protected readonly IUserContext UserContext;

    protected CourseBoardService(ICourseEditionProvider courseEditionProvider, IUserContext userContext)
    {
        CourseEditionProvider = courseEditionProvider;
        UserContext = userContext;
    }

    protected async Task ValidateUserAccessToEditionAsync(Guid editionId, string userId)
    {
        var isAdmin = UserContext.GetUserRole() is UserRole.Admin;
        if (isAdmin)
            return;

        var isRegistered = CourseEditionProvider.IsUserRegisteredToCourseEditionAsync(editionId, userId);
        if (!await isRegistered)
            throw new UnauthorizedAccessException("User is not registered to course edition.");
    }

    protected async Task ValidateWriteAccessToEditionAsync(Guid editionId)
    {
        var courseEdition = await CourseEditionProvider.GetCourseEditionAsync(editionId);
        if (courseEdition is null)
            throw new KeyNotFoundException("Course edition not found.");
        
        if (courseEdition.Status is not CourseEditionStatus.InProgress)
            throw new InvalidOperationException("Cannot modify course edition that is not in progress.");
    }
}