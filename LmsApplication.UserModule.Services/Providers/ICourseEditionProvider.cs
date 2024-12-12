namespace LmsApplication.UserModule.Services.Providers;

public interface ICourseEditionProvider
{
    Task<List<string>> GetCourseEditionParticipantsAsync(Guid courseEditionId);
    
    Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId);
    
    Task<bool> IsUserParticipantOfAnyCourseEditionAsync(string userId);
}