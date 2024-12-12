using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Mapping;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface ICommentService
{
    Task<CollectionResource<CommentModel>> GetCommentsForPostAsync(Guid editionId, Guid postId, int page, int pageSize);
    
    Task<CommentModel> CreateCommentAsync(Guid editionId, Guid postId, CommentCreateModel model);
    
    Task<CommentModel> UpdateCommentAsync(Guid editionId, Guid commentId, CommentUpdateModel model);
    
    Task DeleteCommentAsync(Guid editionId, Guid commentId);
}

public class CommentService : CourseBoardService, ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IValidationService<CreateCommentValidationModel> _commentCreateModelValidationService;
    private readonly IValidationService<UpdateCommentValidationModel> _commentUpdateModelValidationService;
    private readonly IUserProvider _userProvider;
    private readonly IPostRepository _postRepository;

    public CommentService(
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext,
        ICommentRepository commentRepository,
        IValidationService<CreateCommentValidationModel> commentCreateModelValidationService,
        IValidationService<UpdateCommentValidationModel> commentUpdateModelValidationService,
        IUserProvider userProvider,
        IPostRepository postRepository) : base(courseEditionProvider, userContext)
    {
        _commentRepository = commentRepository;
        _commentCreateModelValidationService = commentCreateModelValidationService;
        _commentUpdateModelValidationService = commentUpdateModelValidationService;
        _userProvider = userProvider;
        _postRepository = postRepository;
    }

    public async Task<CollectionResource<CommentModel>> GetCommentsForPostAsync(Guid editionId, Guid postId, int page, int pageSize)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        var (totalCount, comments) = await _commentRepository.GetCommentsForPostAsync(postId, page, pageSize);
        
        var userIds = comments.Select(x => x.UserId)
            .Concat(comments.SelectMany(x => x.Reactions.Select(r => r.UserId)))
            .Distinct()
            .ToList();
        
        var users = await _userProvider.GetUsersByIdsAsync(userIds);
        var usernames = users.ToDictionary(x => x.Key, x => x.Value.Name + "" + x.Value.Surname);
        
        return new CollectionResource<CommentModel>(comments.Select(x => x.ToModel(users[x.UserId], usernames, userId)), totalCount);
    }

    public async Task<CommentModel> CreateCommentAsync(Guid editionId, Guid postId, CommentCreateModel model)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        await ValidateWriteAccessToEditionAsync(editionId);
        
        var validationModel = new CreateCommentValidationModel
        {
            Content = model.Content,
            User = await _userProvider.GetUserByIdAsync(userId),
            Post = await _postRepository.GetPostByIdAsync(postId),
        };
        await _commentCreateModelValidationService.ValidateAndThrowAsync(validationModel);

        var comment = new Comment
        {
            Content = model.Content,
            PostId = postId,
            UserId = userId,
        };
        
        await _commentRepository.CreateCommentAsync(comment);
        
        return comment.ToModel(validationModel.User!, [], userId);
    }

    public async Task<CommentModel> UpdateCommentAsync(Guid editionId, Guid commentId, CommentUpdateModel model)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        await ValidateWriteAccessToEditionAsync(editionId);

        var validationModel = new UpdateCommentValidationModel
        {
            Content = model.Content,
            User = await _userProvider.GetUserByIdAsync(userId),
            Comment = await _commentRepository.GetCommentByIdAsync(commentId),
        };
        await _commentUpdateModelValidationService.ValidateAndThrowAsync(validationModel);
        
        validationModel.Comment!.Content = model.Content;
        
        await _commentRepository.UpdateCommentAsync(validationModel.Comment!);
        
        return validationModel.Comment.ToModel(validationModel.User!, [], userId);
    }

    public async Task DeleteCommentAsync(Guid editionId, Guid commentId)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        await ValidateWriteAccessToEditionAsync(editionId);

        var comment = await _commentRepository.GetCommentByIdAsync(commentId);
        if (comment is null)
            throw new KeyNotFoundException("Comment not found.");
        
        if (comment.UserId != userId && UserContext.GetUserRole() is not UserRole.Admin)
            throw new UnauthorizedAccessException("You are not allowed to delete this comment.");
        
        await _commentRepository.DeleteCommentAsync(comment);
    }
}