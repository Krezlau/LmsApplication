using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseModule.Services.Providers;

public interface ICourseBoardProvider
{
    Task<Dictionary<Guid, FinalGradeModel>> GetFinalGradesDictionaryAsync(string userId);
}