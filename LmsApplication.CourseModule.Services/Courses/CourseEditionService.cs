using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.CourseModule.Data.Mapping;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Courses;

public interface ICourseEditionService
{
    Task<List<CourseEditionModel>> GetAllCourseEditionsAsync();
    
    Task<List<CourseEditionModel>> GetCourseEditionsByCourseIdAsync(Guid courseId);
    
    Task<CourseEditionModel> GetCourseEditionByIdAsync(Guid id);
    
    Task<CourseEditionModel> CreateCourseEditionAsync(CourseEditionPostModel model);
    
    Task AddTeacherToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model);
    
    Task AddStudentToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model);
    
    Task<List<CourseEditionModel>> GetUserCourseEditionsAsync(string userEmail);
}

public class CourseEditionService : ICourseEditionService
{
    private readonly ICourseEditionRepository _courseEditionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IValidationService<CourseEditionPostModel> _courseEditionPostModelValidationService;
    private readonly IValidationService<CourseEditionAddUserModel> _courseEditionAddUserModelValidationService;

    public CourseEditionService(ICourseEditionRepository courseEditionRepository,
        ICourseRepository courseRepository,
        IValidationService<CourseEditionPostModel> courseEditionPostModelValidationService,
        IValidationService<CourseEditionAddUserModel> courseEditionAddUserModelValidationService)
    {
        _courseEditionRepository = courseEditionRepository;
        _courseRepository = courseRepository;
        _courseEditionPostModelValidationService = courseEditionPostModelValidationService;
        _courseEditionAddUserModelValidationService = courseEditionAddUserModelValidationService;
    }

    public async Task<List<CourseEditionModel>> GetAllCourseEditionsAsync()
    {
        var courseEditions = await _courseEditionRepository.GetAllCourseEditionsAsync();

        return courseEditions.Select(x => x.ToModel()).ToList();
    }

    public async Task<List<CourseEditionModel>> GetCourseEditionsByCourseIdAsync(Guid courseId)
    {
        var courseEditions = await _courseEditionRepository.GetCourseEditionsByCourseIdAsync(courseId);
        
        return courseEditions.Select(x => x.ToModel()).ToList();
    }

    public async Task<CourseEditionModel> GetCourseEditionByIdAsync(Guid id)
    {
        var courseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(id);
        if (courseEdition is null)
            throw new KeyNotFoundException("Course edition not found");

        return courseEdition.ToModel();
    }

    public async Task<CourseEditionModel> CreateCourseEditionAsync(CourseEditionPostModel model)
    {
        var course = await _courseRepository.GetCourseByIdAsync(model.CourseId);
        
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
        
        await _courseEditionRepository.CreateAsync(courseEdition);
        
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
        
        await _courseEditionRepository.AddParticipantToCourseEditionAsync(courseId, model.UserEmail, UserRole.Teacher);
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
        
        await _courseEditionRepository.AddParticipantToCourseEditionAsync(courseId, model.UserEmail, UserRole.Student);
    }

    public async Task<List<CourseEditionModel>> GetUserCourseEditionsAsync(string userEmail)
    {
        var courseEditions = await _courseEditionRepository.GetUserCourseEditionsAsync(userEmail);
        
        return courseEditions.Select(x => x.ToModel()).ToList();
    }
}