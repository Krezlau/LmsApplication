namespace LmsApplication.ResourceModule.Services.Providers;

public interface ICourseEditionProvider
{
    Task<bool> CourseEditionExistsAsync(Guid courseEditionId);
    
    Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId);
}