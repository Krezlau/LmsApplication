using LmsApplication.Core.Shared.Providers;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.CourseModule.Data.Mapping;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Courses;

public interface ICourseEditionSettingsService
{
    Task<CourseEditionSettingsModel> GetCourseEditionSettingsAsync(Guid editionId, string userId);
    
    Task UpdateCourseEditionSettingsAsync(Guid editionId, string userId, CourseEditionSettingsUpdateModel model);
}

public class CourseEditionSettingsService : ICourseEditionSettingsService
{
    private readonly ICourseEditionSettingsRepository _courseEditionSettingsRepository;
    private readonly ICourseEditionRepository _courseEditionRepository;
    private readonly IUserProvider _userProvider;

    public CourseEditionSettingsService(
        ICourseEditionSettingsRepository courseEditionSettingsRepository,
        ICourseEditionRepository courseEditionRepository,
        IUserProvider userProvider)
    {
        _courseEditionSettingsRepository = courseEditionSettingsRepository;
        _courseEditionRepository = courseEditionRepository;
        _userProvider = userProvider;
    }

    public async Task<CourseEditionSettingsModel> GetCourseEditionSettingsAsync(Guid editionId, string userId)
    {
        await ValidateUserAccessAsync(editionId, userId);
        
        var settings = await _courseEditionSettingsRepository.GetCourseEditionSettingsAsync(editionId) ??
                       new CourseEditionSettings { CourseEditionId = editionId }; 

        return settings.ToModel();
    }

    public async Task UpdateCourseEditionSettingsAsync(Guid editionId, string userId, CourseEditionSettingsUpdateModel model)
    {
        await ValidateUserAccessAsync(editionId, userId);
        
        var settings = await _courseEditionSettingsRepository.GetCourseEditionSettingsAsync(editionId) ??
                       new CourseEditionSettings { CourseEditionId = editionId };
        
        settings.AllowAllToPost = model.AllowAllToPost;
        
        if (settings.Id == Guid.Empty)
            await _courseEditionSettingsRepository.AddCourseEditionSettingsAsync(settings);
        else
            await _courseEditionSettingsRepository.UpdateCourseEditionSettingsAsync(settings);
    }
    
    private async Task ValidateUserAccessAsync(Guid editionId, string userId)
    {
        var isParticipant = _courseEditionRepository.IsUserParticipantInCourseEditionAsync(editionId, userId);
        var isAdmin = _userProvider.IsUserAdminAsync(userId);
        
        if (!await isParticipant && !await isAdmin)
            throw new UnauthorizedAccessException("User is not a participant of this course edition.");
    }
}