using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Providers;
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
    
    Task RegisterToCourseEditionAsync(Guid courseId, string userId);
    
    Task AddUserToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model);
    
    Task<List<CourseEditionModel>> GetUserCourseEditionsAsync(string userEmail);
}

public class CourseEditionService : ICourseEditionService
{
    private readonly ICourseEditionRepository _courseEditionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IValidationService<CourseEditionPostModel> _courseEditionPostModelValidationService;
    private readonly IValidationService<CourseEditionAddUserModel> _courseEditionAddUserModelValidationService;
    private readonly IValidationService<CourseEditionRegisterModel> _courseEditionRegisterModelValidationService;
    private readonly IUserProvider _userProvider;

    public CourseEditionService(
        ICourseEditionRepository courseEditionRepository,
        ICourseRepository courseRepository,
        IValidationService<CourseEditionPostModel> courseEditionPostModelValidationService,
        IValidationService<CourseEditionAddUserModel> courseEditionAddUserModelValidationService,
        IValidationService<CourseEditionRegisterModel> courseEditionRegisterModelValidationService,
        IUserProvider userProvider)
    {
        _courseEditionRepository = courseEditionRepository;
        _courseRepository = courseRepository;
        _courseEditionPostModelValidationService = courseEditionPostModelValidationService;
        _courseEditionAddUserModelValidationService = courseEditionAddUserModelValidationService;
        _userProvider = userProvider;
        _courseEditionRegisterModelValidationService = courseEditionRegisterModelValidationService;
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
            Title = model.Title,
            StartDateUtc = model.StartDateUtc,
            StudentLimit = model.StudentLimit,
            RegistrationStartDateUtc = model.RegistrationStartDateUtc,
            RegistrationEndDateUtc = model.RegistrationEndDateUtc,
            Duration = course!.Duration,
            Course = course,
        };
        
        await _courseEditionRepository.CreateAsync(courseEdition);
        
        return courseEdition.ToModel();
    }

    public async Task RegisterToCourseEditionAsync(Guid courseId, string userId)
    {
        var course = await _courseEditionRepository.GetCourseEditionByIdAsync(courseId);
        var user = await _userProvider.GetUserByIdAsync(userId);

        await _courseEditionRegisterModelValidationService.ValidateAndThrowAsync(new CourseEditionRegisterModel
        {
            CourseEdition = course,
            User = user,
        });
        
        await _courseEditionRepository.AddParticipantToCourseEditionAsync(courseId, user!.Email, user.Role);
    }

    public async Task AddUserToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model)
    {
        var user = await _userProvider.GetUserByEmailAsync(model.UserEmail);
        var context = new ValidationContext<CourseEditionAddUserModel>(model)
        {
            RootContextData =
            {
                [nameof(courseId)] = courseId,
                [nameof(user)] = user,
            }
        };
        await _courseEditionAddUserModelValidationService.ValidateAndThrowAsync(context);
        
        await _courseEditionRepository.AddParticipantToCourseEditionAsync(courseId, model.UserEmail, user!.Role);
    }

    public async Task<List<CourseEditionModel>> GetUserCourseEditionsAsync(string userEmail)
    {
        var courseEditions = await _courseEditionRepository.GetUserCourseEditionsAsync(userEmail);
        
        return courseEditions.Select(x => x.ToModel()).ToList();
    }
}