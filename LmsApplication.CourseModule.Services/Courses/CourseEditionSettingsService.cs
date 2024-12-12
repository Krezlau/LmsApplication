using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.CourseModule.Data.Mapping;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Courses;

public interface ICourseEditionSettingsService
{
    Task<CourseEditionSettingsModel> GetCourseEditionSettingsAsync(Guid editionId);
    
    Task UpdateCourseEditionSettingsAsync(Guid editionId, CourseEditionSettingsUpdateModel model);
}

public class CourseEditionSettingsService : ICourseEditionSettingsService
{
    private readonly ICourseEditionSettingsRepository _courseEditionSettingsRepository;
    private readonly ICourseEditionRepository _courseEditionRepository;
    private readonly IUserContext _userContext;

    public CourseEditionSettingsService(
        ICourseEditionSettingsRepository courseEditionSettingsRepository,
        ICourseEditionRepository courseEditionRepository,
        IUserContext userContext)
    {
        _courseEditionSettingsRepository = courseEditionSettingsRepository;
        _courseEditionRepository = courseEditionRepository;
        _userContext = userContext;
    }

    public async Task<CourseEditionSettingsModel> GetCourseEditionSettingsAsync(Guid editionId)
    {
        await ValidateUserAccessAsync(editionId);
        
        var settings = await _courseEditionSettingsRepository.GetCourseEditionSettingsAsync(editionId) ??
                       new CourseEditionSettings { CourseEditionId = editionId }; 

        return settings.ToModel();
    }

    public async Task UpdateCourseEditionSettingsAsync(Guid editionId, CourseEditionSettingsUpdateModel model)
    {
        await ValidateUserAccessAsync(editionId);
        
        var courseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(editionId);
        if (courseEdition is null)
            throw new ArgumentException("Course edition not found.");
        
        if (courseEdition.Status is CourseEditionStatus.Finished)
            throw new InvalidOperationException("Cannot change settings of a finished course edition.");
        
        var settings = await _courseEditionSettingsRepository.GetCourseEditionSettingsAsync(editionId) ??
                       new CourseEditionSettings { CourseEditionId = editionId };
        
        settings.AllowAllToPost = model.AllowAllToPost;
        
        if (settings.Id == Guid.Empty)
            await _courseEditionSettingsRepository.AddCourseEditionSettingsAsync(settings);
        else
            await _courseEditionSettingsRepository.UpdateCourseEditionSettingsAsync(settings);
    }
    
    private async Task ValidateUserAccessAsync(Guid editionId)
    {
        var userId = _userContext.GetUserId();
        var isParticipant = _courseEditionRepository.IsUserParticipantInCourseEditionAsync(editionId, userId);
        var isAdmin = _userContext.GetUserRole() is UserRole.Admin;
        
        if (!await isParticipant && !isAdmin)
            throw new UnauthorizedAccessException("User is not a participant of this course edition.");
    }
}