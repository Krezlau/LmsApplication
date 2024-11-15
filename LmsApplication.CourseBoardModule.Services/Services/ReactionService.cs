using LmsApplication.Core.Shared.Providers;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IReactionService
{
    Task<ReactionUpdateModel> UpdatePostReactionAsync(Guid editionId, string userId, Guid postId, ReactionUpdateModel model);
    
    Task<ReactionUpdateModel> UpdateCommentReactionAsync(Guid editionId, string userId, Guid commentId, ReactionUpdateModel model);
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

    public async Task<ReactionUpdateModel> UpdatePostReactionAsync(Guid editionId, string userId, Guid postId, ReactionUpdateModel model)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);

        var currentReaction = await _reactionRepository.GetPostReactionAsync(postId, userId);
        
        var newReaction = model.Type;
        if (currentReaction is null && newReaction is not null)
        {
            await _reactionRepository.CreatePostReactionAsync(new PostReaction
            {
                UserId = userId,
                PostId = postId,
                ReactionType = newReaction.Value,
            });
        }

        if (currentReaction is not null && newReaction is null)
        {
            await _reactionRepository.DeletePostReactionAsync(currentReaction);
        }

        if (currentReaction is not null && newReaction is not null)
        {
            currentReaction.ReactionType = newReaction.Value;
            await _reactionRepository.UpdatePostReactionAsync(currentReaction);
        }

        return model;
    }

    public async Task<ReactionUpdateModel> UpdateCommentReactionAsync(Guid editionId, string userId, Guid commentId, ReactionUpdateModel model)
    {
        await ValidateUserAccessToEditionAsync(editionId, userId);

        var currentReaction = await _reactionRepository.GetCommentReactionAsync(commentId, userId);
        
        var newReaction = model.Type;
        if (currentReaction is null && newReaction is not null)
        {
            await _reactionRepository.CreateCommentReactionAsync(new CommentReaction
            {
                UserId = userId,
                CommentId = commentId,
                ReactionType = newReaction.Value,
            });
        }

        if (currentReaction is not null && newReaction is null)
        {
            await _reactionRepository.DeleteCommentReactionAsync(currentReaction);
        }

        if (currentReaction is not null && newReaction is not null)
        {
            currentReaction.ReactionType = newReaction.Value;
            await _reactionRepository.UpdateCommentReactionAsync(currentReaction);
        }

        return model;
    }
}