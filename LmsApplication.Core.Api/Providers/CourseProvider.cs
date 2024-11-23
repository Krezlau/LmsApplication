using LmsApplication.CourseModule.Services.Repositories;
using LmsApplication.ResourceModule.Services.Providers;

namespace LmsApplication.Core.Api.Providers;

public class CourseProvider : ICourseProvider
{
    private readonly ICourseRepository _repository;

    public CourseProvider(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CourseExistsAsync(Guid courseId)
    {
        return await _repository.CourseExistsAsync(courseId);
    }
}