using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Courses.Validation;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.CourseModule.Data.Mapping;
using LmsApplication.CourseModule.Services.Providers;
using LmsApplication.CourseModule.Services.Repositories;

namespace LmsApplication.CourseModule.Services.Courses;

public interface ICourseEditionService
{
    Task<List<CourseEditionModel>> GetAllCourseEditionsAsync();
    
    Task<List<CourseEditionModel>> GetCourseEditionsByCourseIdAsync(Guid courseId);
    
    Task<CourseEditionModel> GetCourseEditionByIdAsync(Guid id);
    
    Task<CourseEditionModel> CreateCourseEditionAsync(CourseEditionPostModel model);
    
    Task RegisterToCourseEditionAsync(Guid courseId);
    
    Task AddUserToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model);

    Task RemoveUserFromCourseEditionAsync(Guid courseId, CourseEditionRemoveUserModel model);
    
    Task<List<CourseEditionModel>> GetUserCourseEditionsAsync();

    Task<List<CourseEditionModel>> GetEditionsWithRegistrationOpenAsync();
    
    Task<List<CourseEditionModel>> GetCourseEditionsByUserIdAsync(string userId);
    
    Task<List<CourseEditionModel>> GetMutualCourseEditionsAsync(string userId);
}

public class CourseEditionService : ICourseEditionService
{
    private readonly ICourseEditionRepository _courseEditionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IValidationService<CreateCourseEditionValidationModel> _courseEditionPostModelValidationService;
    private readonly IValidationService<CourseEditionAddUserValidationModel> _courseEditionAddUserModelValidationService;
    private readonly IValidationService<CourseEditionRegisterModel> _courseEditionRegisterModelValidationService;
    private readonly IValidationService<CourseEditionRemoveUserValidationModel> _courseEditionRemoveUserModelValidationService;
    private readonly IUserProvider _userProvider;
    private readonly IUserContext _userContext;
    private readonly IQueueClient<CourseEnrollmentNotificationQueueMessage> _queueClient;

    public CourseEditionService(
        ICourseEditionRepository courseEditionRepository,
        ICourseRepository courseRepository,
        IValidationService<CreateCourseEditionValidationModel> courseEditionPostModelValidationService,
        IValidationService<CourseEditionAddUserValidationModel> courseEditionAddUserModelValidationService,
        IValidationService<CourseEditionRegisterModel> courseEditionRegisterModelValidationService,
        IUserProvider userProvider,
        IValidationService<CourseEditionRemoveUserValidationModel> courseEditionRemoveUserModelValidationService,
        IUserContext userContext,
        IQueueClient<CourseEnrollmentNotificationQueueMessage> queueClient)
    {
        _courseEditionRepository = courseEditionRepository;
        _courseRepository = courseRepository;
        _courseEditionPostModelValidationService = courseEditionPostModelValidationService;
        _courseEditionAddUserModelValidationService = courseEditionAddUserModelValidationService;
        _userProvider = userProvider;
        _courseEditionRemoveUserModelValidationService = courseEditionRemoveUserModelValidationService;
        _userContext = userContext;
        _queueClient = queueClient;
        _courseEditionRegisterModelValidationService = courseEditionRegisterModelValidationService;
    }

    public async Task<List<CourseEditionModel>> GetAllCourseEditionsAsync()
    {
        var courseEditions = await _courseEditionRepository.GetAllCourseEditionsAsync();

        return courseEditions.Select(x => x.ToModel(_userContext.GetUserId())).ToList();
    }

    public async Task<List<CourseEditionModel>> GetCourseEditionsByCourseIdAsync(Guid courseId)
    {
        var courseEditions = await _courseEditionRepository.GetCourseEditionsByCourseIdAsync(courseId);
        
        return courseEditions.Select(x => x.ToModel(_userContext.GetUserId())).ToList();
    }

    public async Task<CourseEditionModel> GetCourseEditionByIdAsync(Guid id)
    {
        var courseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(id);
        if (courseEdition is null)
            throw new KeyNotFoundException("Course edition not found");

        return courseEdition.ToModel(_userContext.GetUserId());
    }

    public async Task<CourseEditionModel> CreateCourseEditionAsync(CourseEditionPostModel model)
    {
        var validationModel = new CreateCourseEditionValidationModel
        {
            Title = model.Title,
            CourseId = model.CourseId,
            StartDateUtc = model.StartDateUtc,
            StudentLimit = model.StudentLimit,
            RegistrationStartDateUtc = model.RegistrationStartDateUtc,
            RegistrationEndDateUtc = model.RegistrationEndDateUtc,
            Course = await _courseRepository.GetCourseByIdAsync(model.CourseId),
        };
        
        await _courseEditionPostModelValidationService.ValidateAndThrowAsync(validationModel);
        
        var courseEdition = new CourseEdition
        {
            CourseId = model.CourseId,
            Title = model.Title,
            StartDateUtc = model.StartDateUtc,
            StudentLimit = model.StudentLimit,
            RegistrationStartDateUtc = model.RegistrationStartDateUtc,
            RegistrationEndDateUtc = model.RegistrationEndDateUtc,
            Duration = validationModel.Course!.Duration,
            Course = validationModel.Course,
        };
        
        await _courseEditionRepository.CreateAsync(courseEdition);
        
        return courseEdition.ToModel(_userContext.GetUserId());
    }

    public async Task RegisterToCourseEditionAsync(Guid courseId)
    {
        var courseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(courseId);
        var user = await _userProvider.GetUserByIdAsync(_userContext.GetUserId());

        await _courseEditionRegisterModelValidationService.ValidateAndThrowAsync(new CourseEditionRegisterModel
        {
            CourseEdition = courseEdition,
            User = user,
        });
        
        await _courseEditionRepository.AddParticipantToCourseEditionAsync(courseId, user!.Id, user.Role);
        
        await _queueClient.EnqueueAsync(new CourseEnrollmentNotificationQueueMessage
        {
            User = user,
            CourseName = courseEdition!.Course!.Title,
            CourseEditionId = courseEdition.Id,
            CourseEditionName = courseEdition.Title,
            TimeStampUtc = DateTime.UtcNow,
        });
    }

    public async Task AddUserToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model)
    {
        var validationModel = new CourseEditionAddUserValidationModel
        {
            User = await _userProvider.GetUserByIdAsync(model.UserId),
            CourseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(courseId),
        };
        await _courseEditionAddUserModelValidationService.ValidateAndThrowAsync(validationModel);
        
        await _courseEditionRepository.AddParticipantToCourseEditionAsync(courseId, validationModel.User!.Id,
            validationModel.User.Role);
        
        await _queueClient.EnqueueAsync(new CourseEnrollmentNotificationQueueMessage
        {
            User = validationModel.User,
            CourseName = validationModel.CourseEdition!.Course!.Title,
            CourseEditionId = validationModel.CourseEdition.Id,
            CourseEditionName = validationModel.CourseEdition.Title,
            TimeStampUtc = DateTime.UtcNow,
        });
    }

    public async Task RemoveUserFromCourseEditionAsync(Guid courseId, CourseEditionRemoveUserModel model)
    {
        var validationModel = new CourseEditionRemoveUserValidationModel
        {
            CourseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(courseId),
            User = await _userProvider.GetUserByIdAsync(model.UserId),
        };
        await _courseEditionRemoveUserModelValidationService.ValidateAndThrowAsync(validationModel);
        
        await _courseEditionRepository.RemoveParticipantFromCourseEditionAsync(courseId, validationModel.User!.Id);
    }

    public async Task<List<CourseEditionModel>> GetUserCourseEditionsAsync()
    {
        var userId = _userContext.GetUserId();
        var courseEditions = await _courseEditionRepository.GetUserCourseEditionsAsync(userId);
        
        return courseEditions.Select(x => x.ToModel(userId)).ToList();
    }

    public async Task<List<CourseEditionModel>> GetEditionsWithRegistrationOpenAsync()
    {
        var userId = _userContext.GetUserId();
        var courseEditions = await _courseEditionRepository.GetEditionsWithRegistrationOpenAsync(userId);
        
        return courseEditions.Select(x => x.ToModel(userId)).ToList();
    }

    public async Task<List<CourseEditionModel>> GetCourseEditionsByUserIdAsync(string userId)
    {
        var courseEditions = await _courseEditionRepository.GetCourseEditionsByUserIdAsync(userId);
        
        return courseEditions.Select(x => x.ToModel(userId)).ToList();
    }

    public async Task<List<CourseEditionModel>> GetMutualCourseEditionsAsync(string userId)
    {
        var courseEditions = await _courseEditionRepository.GetMutualCourseEditionsAsync(userId, _userContext.GetUserId());
        
        return courseEditions.Select(x => x.ToModel(userId)).ToList();
    }
}