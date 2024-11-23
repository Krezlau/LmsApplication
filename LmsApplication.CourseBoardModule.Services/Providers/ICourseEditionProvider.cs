using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Services.Providers;

public interface ICourseEditionProvider
{
    Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId);
    
    Task<CourseEditionPublicSettingsModel> GetCourseEditionPublicSettingsAsync(Guid courseEditionId);
}