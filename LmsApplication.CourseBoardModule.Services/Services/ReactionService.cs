using LmsApplication.Core.Shared.Providers;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IReactionService
{
    Task<ReactionType?> UpdatePostReactionAsync(Guid editionId, string userId, Guid postId, ReactionType? type);
    
    Task<ReactionType?> UpdateCommentReactionAsync(Guid editionId, string userId, Guid commentId, ReactionType? type);
}

public class ReactionService : CourseBoardService, IReactionService
{
    private readonly IReactionRepository _reactionRepository;
    
    public ReactionService( 
        ICourseEditionProvider courseEditionProvider,
        IUserProvider userProvider,
        IReactionRepository reactionRepository) : base(courseEditionProvider, userProvider)
    {
        _reactionRepository = reactionRepository;
    }

    public async Task<ReactionType?> UpdatePostReactionAsync(Guid editionId, string userId, Guid postId, ReactionType? type)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);

        var currentReaction = await _reactionRepository.GetPostReactionAsync(postId, userId);

        if (currentReaction is null && type is not null)
        {
            await _reactionRepository.CreatePostReactionAsync(new PostReaction
            {
                UserId = userId,
                PostId = postId,
                ReactionType = type.Value,
            });
        }

        if (currentReaction is not null && type is null)
        {
            await _reactionRepository.DeletePostReactionAsync(currentReaction);
        }

        if (currentReaction is not null && type is not null)
        {
            currentReaction.ReactionType = type.Value;
            await _reactionRepository.UpdatePostReactionAsync(currentReaction);
        }

        return type;
    }

    public async Task<ReactionType?> UpdateCommentReactionAsync(Guid editionId, string userId, Guid commentId, ReactionType? type)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);

        var currentReaction = await _reactionRepository.GetCommentReactionAsync(commentId, userId);

        if (currentReaction is null && type is not null)
        {
            await _reactionRepository.CreateCommentReactionAsync(new CommentReaction
            {
                UserId = userId,
                CommentId = commentId,
                ReactionType = type.Value,
            });
        }

        if (currentReaction is not null && type is null)
        {
            await _reactionRepository.DeleteCommentReactionAsync(currentReaction);
        }

        if (currentReaction is not null && type is not null)
        {
            currentReaction.ReactionType = type.Value;
            await _reactionRepository.UpdateCommentReactionAsync(currentReaction);
        }

        return type;
    }
}