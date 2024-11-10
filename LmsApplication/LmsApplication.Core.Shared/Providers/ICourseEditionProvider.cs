namespace LmsApplication.Core.Shared.Providers;

public interface ICourseEditionProvider
{
    Task<List<string>> GetCourseEditionParticipantsAsync(Guid courseEditionId);
    
    Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId);
}