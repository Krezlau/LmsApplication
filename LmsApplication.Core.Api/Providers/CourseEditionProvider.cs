using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.Core.Api.Providers;

public class CourseEditionProvider : 
    CourseBoardModule.Services.Providers.ICourseEditionProvider,
    ResourceModule.Services.Providers.ICourseEditionProvider,
    UserModule.Services.Providers.ICourseEditionProvider
{
    private readonly ICourseEditionRepository _courseEditionRepository;
    private readonly ICourseEditionSettingsRepository _courseEditionSettingsRepository;

    public CourseEditionProvider(
        ICourseEditionRepository courseEditionRepository,
        ICourseEditionSettingsRepository courseEditionSettingsRepository)
    {
        _courseEditionRepository = courseEditionRepository;
        _courseEditionSettingsRepository = courseEditionSettingsRepository;
    }

    public async Task<bool> CourseEditionExistsAsync(Guid courseEditionId)
    {
        return await _courseEditionRepository.CourseEditionExistsAsync(courseEditionId);
    }

    public async Task<List<string>> GetCourseEditionParticipantsAsync(Guid courseEditionId)
    {
        return await _courseEditionRepository.GetCourseEditionParticipantIdsAsync(courseEditionId);
    }

    public async Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId)
    {
        return await _courseEditionRepository.IsUserParticipantInCourseEditionAsync(courseEditionId, userId);
    }

    public async Task<CourseEditionPublicSettingsModel> GetCourseEditionPublicSettingsAsync(Guid courseEditionId)
    {
        var settings = await _courseEditionSettingsRepository.GetCourseEditionSettingsAsync(courseEditionId);

        return new CourseEditionPublicSettingsModel { AllowAllToPost = settings?.AllowAllToPost ?? new CourseEditionSettings().AllowAllToPost };
    }
}