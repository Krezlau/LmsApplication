using LmsApplication.Core.Data.Entities;

namespace LmsApplication.Core.Services.Courses;

public interface ICourseEditionService
{
    Task<List<CourseEdition>> GetAllCourseEditionsAsync();
}

public class CourseEditionService : ICourseEditionService
{
    public async Task<List<CourseEdition>> GetAllCourseEditionsAsync()
    {
        throw new NotImplementedException();
    }
}