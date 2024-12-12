using LmsApplication.Core.Shared.Models;
using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseModule.Data.Courses;
using LmsApplication.CourseModule.Data.Courses.Validation;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.CourseModule.Data.Mapping;
using LmsApplication.CourseModule.Services.Providers;
using LmsApplication.CourseModule.Services.Repositories;
using CourseEditionModel = LmsApplication.CourseModule.Data.Courses.CourseEditionModel;

namespace LmsApplication.CourseModule.Services.Courses;

public interface ICourseEditionService
{
    Task<CollectionResource<CourseEditionModel>> GetAllCourseEditionsAsync(int page, int pageSize);
    
    Task<CollectionResource<CourseEditionModel>> GetCourseEditionsByCourseIdAsync(Guid courseId, int page, int pageSize);
    
    Task<CourseEditionModel> GetCourseEditionByIdAsync(Guid id);
    
    Task<CourseEditionModel> CreateCourseEditionAsync(CourseEditionPostModel model);
    
    Task RegisterToCourseEditionAsync(Guid courseId);
    
    Task AddUserToCourseEditionAsync(Guid courseId, CourseEditionAddUserModel model);

    Task RemoveUserFromCourseEditionAsync(Guid courseId, CourseEditionRemoveUserModel model);
    
    Task<CollectionResource<CourseEditionModel>> GetUserCourseEditionsAsync(int page, int pageSize);

    Task<CollectionResource<CourseEditionModel>> GetEditionsWithRegistrationOpenAsync(int page, int pageSize);
    
    Task<CollectionResource<CourseEditionModel>> GetCourseEditionsByUserIdAsync(string userId, int page, int pageSize);
    
    Task<CollectionResource<CourseEditionModel>> GetMutualCourseEditionsAsync(string userId, int page, int pageSize);
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

    public async Task<CollectionResource<CourseEditionModel>> GetAllCourseEditionsAsync(int page, int pageSize)
    {
        var (totalCount, courseEditions)= await _courseEditionRepository.GetAllCourseEditionsAsync(page, pageSize);

        return new CollectionResource<CourseEditionModel>(courseEditions.Select(x => x.ToModel(_userContext.GetUserId())), totalCount);
    }

    public async Task<CollectionResource<CourseEditionModel>> GetCourseEditionsByCourseIdAsync(Guid courseId, int page, int pageSize)
    {
        var (totalCount, courseEditions) = await _courseEditionRepository.GetCourseEditionsByCourseIdAsync(courseId, page, pageSize);
        
        return new CollectionResource<CourseEditionModel>(courseEditions.Select(x => x.ToModel(_userContext.GetUserId())), totalCount);
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

    public async Task<CollectionResource<CourseEditionModel>> GetUserCourseEditionsAsync(int page, int pageSize)
    {
        var userId = _userContext.GetUserId();
        var (totalCount, courseEditions) = await _courseEditionRepository.GetUserCourseEditionsAsync(userId, page, pageSize);
        
        return new CollectionResource<CourseEditionModel>(courseEditions.Select(x => x.ToModel(userId)), totalCount);
    }

    public async Task<CollectionResource<CourseEditionModel>> GetEditionsWithRegistrationOpenAsync(int page, int pageSize)
    {
        var userId = _userContext.GetUserId();
        var (totalCount, courseEditions) = await _courseEditionRepository.GetEditionsWithRegistrationOpenAsync(userId, page, pageSize);
        
        return new CollectionResource<CourseEditionModel>(courseEditions.Select(x => x.ToModel(userId)), totalCount);
    }

    public async Task<CollectionResource<CourseEditionModel>> GetCourseEditionsByUserIdAsync(string userId, int page, int pageSize)
    {
        var (totalCount, courseEditions) = await _courseEditionRepository.GetCourseEditionsByUserIdAsync(userId, page, pageSize);
        
        return new CollectionResource<CourseEditionModel>(courseEditions.Select(x => x.ToModel(userId)), totalCount);
    }

    public async Task<CollectionResource<CourseEditionModel>> GetMutualCourseEditionsAsync(string userId, int page, int pageSize)
    {
        var (totalCount, courseEditions) = await _courseEditionRepository.GetMutualCourseEditionsAsync(userId, _userContext.GetUserId(), page, pageSize);
        
        return new CollectionResource<CourseEditionModel>(courseEditions.Select(x => x.ToModel(userId)), totalCount);
    }
}