using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Mapping;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IPostService
{
    Task<CollectionResource<PostModel>> GetPostsAsync(Guid editionId, int page, int pageSize);
    
    Task<PostModel> CreatePostAsync(Guid editionId, PostCreateModel postCreateModel);
    
    Task<PostModel> UpdatePostAsync(Guid editionId, Guid postId, PostUpdateModel postUpdateModel);
    
    Task DeletePostAsync(Guid editionId, Guid postId);
}

public class PostService : CourseBoardService, IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IValidationService<CreatePostValidationModel> _postCreateModelValidationService;
    private readonly IValidationService<UpdatePostValidationModel> _postUpdateModelValidationService;
    private readonly IUserProvider _userProvider;
    private readonly IQueueClient<PostBatchNotificationQueueMessage> _postBatchNotificationQueueClient;

    public PostService(
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext,
        IPostRepository postRepository,
        IValidationService<CreatePostValidationModel> postCreateModelValidationService,
        IValidationService<UpdatePostValidationModel> postUpdateModelValidationService,
        IUserProvider userProvider,
        IQueueClient<PostBatchNotificationQueueMessage> postBatchNotificationQueueClient) : base(courseEditionProvider, userContext)
    {
        _postRepository = postRepository;
        _postCreateModelValidationService = postCreateModelValidationService;
        _postUpdateModelValidationService = postUpdateModelValidationService;
        _userProvider = userProvider;
        _postBatchNotificationQueueClient = postBatchNotificationQueueClient;
    }

    public async Task<CollectionResource<PostModel>> GetPostsAsync(Guid editionId, int page, int pageSize)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var (totalCount, posts) = await _postRepository.GetPostsAsync(editionId, page, pageSize);
        
        var userIds = posts.Select(x => x.UserId)
            .Concat(posts.SelectMany(x => x.Reactions.Select(r => r.UserId)))
            .Distinct()
            .ToList();
        
        var users = await _userProvider.GetUsersByIdsAsync(userIds);
        var usernames = users.ToDictionary(x => x.Key, x => x.Value.Name + "" + x.Value.Surname);
        
        return new CollectionResource<PostModel>(posts.Select(x => x.ToModel(users[x.UserId], usernames, userId)), totalCount);
    }

    public async Task<PostModel> CreatePostAsync(Guid editionId, PostCreateModel postCreateModel)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToPostAsync(editionId, userId);

        var validationModel = new CreatePostValidationModel
        {
            Content = postCreateModel.Content,
            User = await _userProvider.GetUserByIdAsync(userId),
        };
        await _postCreateModelValidationService.ValidateAndThrowAsync(validationModel);

        var post = new Post
        {
            EditionId = editionId,
            UserId = userId,
            Content = postCreateModel.Content,
        };

        await _postRepository.CreatePostAsync(post);
        
        await _postBatchNotificationQueueClient.EnqueueAsync(new PostBatchNotificationQueueMessage
        {
            CourseEditionId = editionId,
            Poster = validationModel.User!,
            PostBody = post.Content,
            TimeStampUtc = DateTime.UtcNow,
            CourseEditionName = (await CourseEditionProvider.GetCourseEditionAsync(editionId))!.Name,
        });

        return post.ToModel(validationModel.User!, [], userId);
    }

    public async Task<PostModel> UpdatePostAsync(Guid editionId, Guid postId, PostUpdateModel model)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var validationModel = new UpdatePostValidationModel
        {
            Content = model.Content,
            Post = await _postRepository.GetPostByIdAsync(postId),
            User = await _userProvider.GetUserByIdAsync(userId),
        };
        await _postUpdateModelValidationService.ValidateAndThrowAsync(validationModel);
        
        validationModel.Post!.Content = model.Content;
        
        await _postRepository.UpdatePostAsync(validationModel.Post);
        
        return validationModel.Post.ToModel(validationModel.User!, [], userId);
    }

    public async Task DeletePostAsync(Guid editionId, Guid postId)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var post = await _postRepository.GetPostByIdAsync(postId);
        if (post is null) 
            throw new KeyNotFoundException("Post not found.");
        
        if (post.UserId != userId && UserContext.GetUserRole() is not UserRole.Admin)
            throw new UnauthorizedAccessException("User is not allowed to delete this post.");
        
        await _postRepository.DeletePostAsync(post);
    }
    
    private async Task ValidateUserAccessToPostAsync(Guid editionId, string userId)
    {
        var userRole = UserContext.GetUserRole();
        if (userRole is UserRole.Admin)
            return;
        
        var isRegistered = await CourseEditionProvider.IsUserRegisteredToCourseEditionAsync(editionId, userId);
        if (userRole is UserRole.Teacher && isRegistered)
            return;
        
        var settings = await CourseEditionProvider.GetCourseEditionPublicSettingsAsync(editionId);
        if (settings.AllowAllToPost && isRegistered)
            return;
        
        throw new UnauthorizedAccessException("User is not allowed to create posts.");
    }
}