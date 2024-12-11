using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Services.Providers;

public interface ICourseEditionProvider
{
    Task<CourseEditionModel?> GetCourseEditionAsync(Guid courseEditionId);
    
    Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId);
    
    Task<CourseEditionPublicSettingsModel> GetCourseEditionPublicSettingsAsync(Guid courseEditionId);
    
    Task<bool> CourseEditionExistsAsync(Guid courseEditionId);

    Task<List<string>> GetCourseEditionStudentsAsync(Guid courseEditionId);
}