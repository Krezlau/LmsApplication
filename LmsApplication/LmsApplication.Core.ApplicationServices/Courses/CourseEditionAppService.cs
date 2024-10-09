using FluentValidation;
using LmsApplication.Api.Shared.Validation;
using LmsApplication.Core.Data.Entities;
using LmsApplication.Core.Data.Enums;
using LmsApplication.Core.Data.Mapping;
using LmsApplication.Core.Data.Models.Courses;
using LmsApplication.Core.Services.Courses;

namespace LmsApplication.Core.ApplicationServices.Courses;

public interface ICourseEditionAppService
{
    Task<List<CourseEditionModel>> GetAllCourseEditionsAsync();
    
    Task<List<CourseEditionModel>> GetCourseEditionsByCourseIdAsync(Guid courseId);
    
    Task<CourseEditionModel> GetCourseEditionByIdAsync(Guid id);
    
    Task<CourseEditionModel> CreateCourseEditionAsync(CourseEditionPostModel model);
    
    Task AddTeacherToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model);
    
    Task AddStudentToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model);
    
    Task<List<CourseEditionModel>> GetUserCourseEditionsAsync(string userEmail);
}

public class CourseEditionAppService : ICourseEditionAppService
{
    private readonly ICourseEditionService _courseEditionService;
    private readonly ICourseService _courseService;
    private readonly IValidationService<CourseEditionPostModel> _courseEditionPostModelValidationService;
    private readonly IValidationService<CourseEditionAddUserModel> _courseEditionAddUserModelValidationService;

    public CourseEditionAppService(ICourseEditionService courseEditionService,
        ICourseService courseService,
        IValidationService<CourseEditionPostModel> courseEditionPostModelValidationService,
        IValidationService<CourseEditionAddUserModel> courseEditionAddUserModelValidationService)
    {
        _courseEditionService = courseEditionService;
        _courseService = courseService;
        _courseEditionPostModelValidationService = courseEditionPostModelValidationService;
        _courseEditionAddUserModelValidationService = courseEditionAddUserModelValidationService;
    }

    public async Task<List<CourseEditionModel>> GetAllCourseEditionsAsync()
    {
        var courseEditions = await _courseEditionService.GetAllCourseEditionsAsync();

        return courseEditions.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<CourseEditionModel>> GetCourseEditionsByCourseIdAsync(Guid courseId)
    {
        var courseEditions = await _courseEditionService.GetCourseEditionsByCourseIdAsync(courseId);
        
        return courseEditions.Select(x => x.ToModel()).ToList();
    }

    public async Task<CourseEditionModel> GetCourseEditionByIdAsync(Guid id)
    {
        var courseEdition = await _courseEditionService.GetCourseEditionByIdAsync(id);
        if (courseEdition is null)
            throw new KeyNotFoundException("Course edition not found");

        return courseEdition.ToModel();
    }

    public async Task<CourseEditionModel> CreateCourseEditionAsync(CourseEditionPostModel model)
    {
        var course = await _courseService.GetCourseByIdAsync(model.CourseId);
        
        var context = new ValidationContext<CourseEditionPostModel>(model)
        {
            RootContextData =
            {
                [nameof(Course)] = course
            }
        };
        await _courseEditionPostModelValidationService.ValidateAndThrowAsync(context);
        
        var courseEdition = new CourseEdition
        {
            CourseId = model.CourseId,
            StartDateUtc = model.StartDateUtc,
            StudentLimit = model.StudentLimit,
            Duration = course!.Duration,
            Course = course,
        };
        
        await _courseEditionService.CreateAsync(courseEdition);
        
        return courseEdition.ToModel();
    }

    public async Task AddTeacherToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model)
    {
        var context = new ValidationContext<CourseEditionAddUserModel>(model)
        {
            RootContextData =
            {
                [nameof(courseId)] = courseId,
                [nameof(UserRole)] = UserRole.Teacher
            }
        };
        await _courseEditionAddUserModelValidationService.ValidateAndThrowAsync(context);
        
        await _courseEditionService.AddParticipantToCourseEditionAsync(courseId, model.UserEmail, UserRole.Teacher);
    }

    public async Task AddStudentToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model)
    {
        var context = new ValidationContext<CourseEditionAddUserModel>(model)
        {
            RootContextData =
            {
                [nameof(courseId)] = courseId,
                [nameof(UserRole)] = UserRole.Student
            }
        };
        await _courseEditionAddUserModelValidationService.ValidateAndThrowAsync(context);
        
        await _courseEditionService.AddParticipantToCourseEditionAsync(courseId, model.UserEmail, UserRole.Student);
    }

    public async Task<List<CourseEditionModel>> GetUserCourseEditionsAsync(string userEmail)
    {
        var courseEditions = await _courseEditionService.GetUserCourseEditionsAsync(userEmail);
        
        return courseEditions.Select(x => x.ToModel()).ToList();
    }
}