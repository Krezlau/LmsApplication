namespace LmsApplication.ResourceModule.Services.Providers;

public interface ICourseProvider
{
    Task<bool> CourseExistsAsync(Guid courseId);
}