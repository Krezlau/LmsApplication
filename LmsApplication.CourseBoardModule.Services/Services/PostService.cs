using LmsApplication.Core.Shared.Providers;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Mapping;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IPostService
{
    Task<CollectionResource<PostModel>> GetPostsAsync(Guid editionId, string userId, int page, int pageSize);
    
    Task<PostModel> CreatePostAsync(Guid editionId, string userId, PostCreateModel postCreateModel);
}

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly ICourseEditionProvider _courseEditionProvider;
    private readonly IUserProvider _userProvider;
    private readonly IValidationService<PostCreateModel> _postCreateModelValidationService;

    public PostService(
        IPostRepository postRepository,
        ICourseEditionProvider courseEditionProvider,
        IUserProvider userProvider,
        IValidationService<PostCreateModel> postCreateModelValidationService)
    {
        _postRepository = postRepository;
        _courseEditionProvider = courseEditionProvider;
        _userProvider = userProvider;
        _postCreateModelValidationService = postCreateModelValidationService;
    }

    public async Task<CollectionResource<PostModel>> GetPostsAsync(Guid editionId, string userId, int page, int pageSize)
    {
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

    public async Task<PostModel> CreatePostAsync(Guid editionId, string userId, PostCreateModel postCreateModel)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var user = await _userProvider.GetUserByIdAsync(userId);
        if (user is null) 
            throw new KeyNotFoundException("User not found.");
        
        await _postCreateModelValidationService.ValidateAndThrowAsync(postCreateModel);

        var post = new Post()
        {
            EditionId = editionId,
            UserId = userId,
            Content = postCreateModel.Content,
        };

        await _postRepository.CreatePostAsync(post);

        return post.ToModel(user, [], userId);
    }
    
    private async Task ValidateUserAccessToEditionAsync(Guid editionId, string userId)
    {
        var isAdmin = _userProvider.IsUserAdminAsync(userId);
        var isRegistered = _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(editionId, userId);
        
        if (!await isRegistered && !await isAdmin)
            throw new UnauthorizedAccessException("User is not registered to course edition.");
    }
}