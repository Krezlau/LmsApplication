using LmsApplication.Core.Shared.Enums;
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

    public async Task<List<string>> GetCourseEditionStudentsAsync(Guid courseEditionId)
    {
        return await _courseEditionRepository.GetCourseEditionParticipantIdsAsync(courseEditionId, UserRole.Student);
    }

    public async Task<List<string>> GetCourseEditionParticipantsAsync(Guid courseEditionId)
    {
        return await _courseEditionRepository.GetCourseEditionParticipantIdsAsync(courseEditionId);
    }

    public async Task<CourseEditionModel?> GetCourseEditionAsync(Guid courseEditionId)
    {
        var courseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(courseEditionId);
        if (courseEdition is null) 
            return null;

        return new CourseEditionModel
        {
            Id = courseEdition.Id,
            Name = courseEdition.Title,
            Status = courseEdition.Status
        };
    }

    public async Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId)
    {
        return await _courseEditionRepository.IsUserParticipantInCourseEditionAsync(courseEditionId, userId);
    }

    public async Task<bool> IsUserParticipantOfAnyCourseEditionAsync(string userId)
    {
        return await _courseEditionRepository.IsUserParticipantInAnyCourseEditionAsync(userId);
    }

    public async Task<CourseEditionPublicSettingsModel> GetCourseEditionPublicSettingsAsync(Guid courseEditionId)
    {
        var settings = await _courseEditionSettingsRepository.GetCourseEditionSettingsAsync(courseEditionId);

        return new CourseEditionPublicSettingsModel { AllowAllToPost = settings?.AllowAllToPost ?? new CourseEditionSettings().AllowAllToPost };
    }
}