using FluentValidation;
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
    
    Task<PostModel> UpdatePostAsync(Guid editionId, string userId, Guid postId, PostUpdateModel postUpdateModel);
    
    Task DeletePostAsync(Guid editionId, string userId, Guid postId);
}

public class PostService : CourseBoardService, IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IValidationService<PostCreateModel> _postCreateModelValidationService;
    private readonly IValidationService<PostUpdateModel> _postUpdateModelValidationService;

    public PostService(
        IPostRepository postRepository,
        ICourseEditionProvider courseEditionProvider,
        IUserProvider userProvider,
        IValidationService<PostCreateModel> postCreateModelValidationService,
        IValidationService<PostUpdateModel> postUpdateModelValidationService) : base(courseEditionProvider, userProvider)
    {
        _postRepository = postRepository;
        _postCreateModelValidationService = postCreateModelValidationService;
        _postUpdateModelValidationService = postUpdateModelValidationService;
    }

    public async Task<CollectionResource<PostModel>> GetPostsAsync(Guid editionId, string userId, int page, int pageSize)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var (totalCount, posts) = await _postRepository.GetPostsAsync(editionId, page, pageSize);
        
        var userIds = posts.Select(x => x.UserId)
            .Concat(posts.SelectMany(x => x.Reactions.Select(r => r.UserId)))
            .Distinct()
            .ToList();
        
        var users = await UserProvider.GetUsersByIdsAsync(userIds);
        var usernames = users.ToDictionary(x => x.Key, x => x.Value.Name + "" + x.Value.Surname);
        
        return new CollectionResource<PostModel>(posts.Select(x => x.ToModel(users[x.UserId], usernames, userId)), totalCount);
    }

    public async Task<PostModel> CreatePostAsync(Guid editionId, string userId, PostCreateModel postCreateModel)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var user = await UserProvider.GetUserByIdAsync(userId);
        if (user is null) 
            throw new KeyNotFoundException("User not found.");
        
        await _postCreateModelValidationService.ValidateAndThrowAsync(postCreateModel);

        var post = new Post
        {
            EditionId = editionId,
            UserId = userId,
            Content = postCreateModel.Content,
        };

        await _postRepository.CreatePostAsync(post);

        return post.ToModel(user, [], userId);
    }

    public async Task<PostModel> UpdatePostAsync(Guid editionId, string userId, Guid postId, PostUpdateModel model)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var post = await _postRepository.GetPostByIdAsync(postId);
        var user = await UserProvider.GetUserByIdAsync(userId);
        
        var context = new ValidationContext<PostUpdateModel>(model)
        {
            RootContextData = 
            {
                { nameof(Post), post },
                { nameof(user), user },
            }
        };
        await _postUpdateModelValidationService.ValidateAndThrowAsync(context);
        
        post!.Content = model.Content;
        
        await _postRepository.UpdatePostAsync(post);
        
        return post.ToModel(user!, [], userId);
    }

    public async Task DeletePostAsync(Guid editionId, string userId, Guid postId)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var post = await _postRepository.GetPostByIdAsync(postId);
        if (post is null) 
            throw new KeyNotFoundException("Post not found.");
        
        if (post.UserId != userId && !await UserProvider.IsUserAdminAsync(userId))
            throw new UnauthorizedAccessException("User is not allowed to delete this post.");
        
        await _postRepository.DeletePostAsync(post);
    }
}