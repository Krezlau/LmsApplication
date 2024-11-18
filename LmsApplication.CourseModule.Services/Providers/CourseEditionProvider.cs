using LmsApplication.Core.Shared.Models;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Services.Providers;

public class CourseEditionProvider : ICourseEditionProvider
{
    private readonly CourseDbContext _courseDbContext;

    public CourseEditionProvider(CourseDbContext courseDbContext)
    {
        _courseDbContext = courseDbContext;
    }

    public async Task<List<string>> GetCourseEditionParticipantsAsync(Guid courseEditionId)
    {
        return await _courseDbContext.CourseEditionParticipants
            .Where(x => x.CourseEditionId == courseEditionId)
            .Select(x => x.ParticipantId)
            .ToListAsync();
    }

    public async Task<bool> IsUserRegisteredToCourseEditionAsync(Guid courseEditionId, string userId)
    {
        return await _courseDbContext.CourseEditionParticipants
            .AnyAsync(x => x.CourseEditionId == courseEditionId && x.ParticipantId == userId);
    }

    public async Task<CourseEditionPublicSettingsModel> GetCourseEditionPublicSettingsAsync(Guid courseEditionId)
    {
        var settings = await _courseDbContext.CourseEditionSettings
            .FirstOrDefaultAsync(x => x.CourseEditionId == courseEditionId);

        return new CourseEditionPublicSettingsModel { AllowAllToPost = settings?.AllowAllToPost ?? new CourseEditionSettings().AllowAllToPost };
    }
}