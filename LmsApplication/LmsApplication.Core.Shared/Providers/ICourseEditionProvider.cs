using LmsApplication.Core.Shared.Models;

namespace LmsApplication.Core.Shared.Providers;

public interface ICourseEditionProvider
{
    Task<List<string>> GetCourseEditionParticipantsAsync(Guid courseEditionId);
    
    Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId);
    
    Task<CourseEditionPublicSettingsModel> GetCourseEditionPublicSettingsAsync(Guid courseEditionId);
}