using LmsApplication.Core.Shared.Models;

namespace LmsApplication.Core.Shared.Providers;

public interface ICourseEditionProvider
{
    Task<bool> CourseEditionExistsAsync(Guid courseEditionId);
    
    Task<List<string>> GetCourseEditionParticipantsAsync(Guid courseEditionId);
    
    Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId);
    
    Task<CourseEditionPublicSettingsModel> GetCourseEditionPublicSettingsAsync(Guid courseEditionId);
}