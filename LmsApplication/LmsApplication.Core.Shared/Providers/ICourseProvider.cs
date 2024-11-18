namespace LmsApplication.Core.Shared.Providers;

public interface ICourseProvider
{
    Task<bool> CourseExistsAsync(Guid courseId);
}