using FluentValidation;
using LmsApplication.Core.Shared.Providers;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Mapping;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface ICommentService
{
    Task<CollectionResource<CommentModel>> GetCommentsForPostAsync(Guid editionId, string userId, Guid postId, int page, int pageSize);
    
    Task<CommentModel> CreateCommentAsync(Guid editionId, string userId, Guid postId, CommentCreateModel model);
    
    Task<CommentModel> UpdateCommentAsync(Guid editionId, string userId, Guid commentId, CommentUpdateModel model);
    
    Task DeleteCommentAsync(Guid editionId, string userId, Guid commentId);
}

public class CommentService : CourseBoardService, ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IValidationService<CommentCreateModel> _commentCreateModelValidationService;
    private readonly IValidationService<CommentUpdateModel> _commentUpdateModelValidationService;
    
    public CommentService(
        ICourseEditionProvider courseEditionProvider,
        IUserProvider userProvider,
        ICommentRepository commentRepository,
        IValidationService<CommentCreateModel> commentCreateModelValidationService,
        IValidationService<CommentUpdateModel> commentUpdateModelValidationService) : base(courseEditionProvider, userProvider)
    {
        _commentRepository = commentRepository;
        _commentCreateModelValidationService = commentCreateModelValidationService;
        _commentUpdateModelValidationService = commentUpdateModelValidationService;
    }

    public async Task<CollectionResource<CommentModel>> GetCommentsForPostAsync(Guid editionId, string userId, Guid postId, int page, int pageSize)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var (totalCount, comments) = await _commentRepository.GetCommentsForPostAsync(postId, page, pageSize);
        
        var userIds = comments.Select(x => x.UserId)
            .Concat(comments.SelectMany(x => x.Reactions.Select(r => r.UserId)))
            .Distinct()
            .ToList();
        
        var users = await UserProvider.GetUsersByIdsAsync(userIds);
        var usernames = users.ToDictionary(x => x.Key, x => x.Value.Name + "" + x.Value.Surname);
        
        return new CollectionResource<CommentModel>(comments.Select(x => x.ToModel(users[x.UserId], usernames, userId)), totalCount);
    }

    public async Task<CommentModel> CreateCommentAsync(Guid editionId, string userId, Guid postId, CommentCreateModel model)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var user = await UserProvider.GetUserByIdAsync(userId);
        if (user is null) 
            throw new KeyNotFoundException("User not found.");
        
        await _commentCreateModelValidationService.ValidateAndThrowAsync(model);

        var comment = new Comment
        {
            Content = model.Content,
            PostId = postId,
            UserId = userId,
        };
        
        await _commentRepository.CreateCommentAsync(comment);
        
        return comment.ToModel(user, [], userId);
    }

    public async Task<CommentModel> UpdateCommentAsync(Guid editionId, string userId, Guid commentId, CommentUpdateModel model)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var comment = await _commentRepository.GetCommentByIdAsync(commentId);
        var user = await UserProvider.GetUserByIdAsync(userId);
        
        var context = new ValidationContext<CommentUpdateModel>(model)
        {
            RootContextData = 
            {
                { nameof(Comment), comment },
                { nameof(user), user},
            }
        };
        await _commentUpdateModelValidationService.ValidateAndThrowAsync(context);
        
        comment!.Content = model.Content;
        
        await _commentRepository.UpdateCommentAsync(comment);
        
        return comment.ToModel(user!, [], userId);
    }

    public async Task DeleteCommentAsync(Guid editionId, string userId, Guid commentId)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);

        var comment = await _commentRepository.GetCommentByIdAsync(commentId);
        if (comment is null)
            throw new KeyNotFoundException("Comment not found.");
        
        if (comment.UserId != userId && !await UserProvider.IsUserAdminAsync(userId))
            throw new UnauthorizedAccessException("You are not allowed to delete this comment.");
        
        await _commentRepository.DeleteCommentAsync(comment);
    }
}