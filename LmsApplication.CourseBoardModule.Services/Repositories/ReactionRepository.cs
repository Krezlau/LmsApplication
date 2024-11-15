using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseBoardModule.Services.Repositories;

public interface IReactionRepository
{
    Task<PostReaction?> GetPostReactionAsync(Guid postId, string userId);

    Task CreatePostReactionAsync(PostReaction reaction);
    
    Task UpdatePostReactionAsync(PostReaction reaction);
    
    Task DeletePostReactionAsync(PostReaction reaction);
    
    Task<CommentReaction?> GetCommentReactionAsync(Guid commentId, string userId);
    
    Task CreateCommentReactionAsync(CommentReaction reaction);
    
    Task UpdateCommentReactionAsync(CommentReaction reaction);
    
    Task DeleteCommentReactionAsync(CommentReaction reaction);
}

public class ReactionRepository : IReactionRepository
{
    private readonly CourseBoardDbContext _dbContext;

    public ReactionRepository(CourseBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PostReaction?> GetPostReactionAsync(Guid postId, string userId)
    {
        return await _dbContext.PostReactions
            .Where(pr => pr.PostId == postId && pr.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task CreatePostReactionAsync(PostReaction reaction)
    {
        await _dbContext.PostReactions.AddAsync(reaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePostReactionAsync(PostReaction reaction)
    {
        _dbContext.PostReactions.Update(reaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePostReactionAsync(PostReaction reaction)
    {
        _dbContext.PostReactions.Remove(reaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<CommentReaction?> GetCommentReactionAsync(Guid commentId, string userId)
    {
        return await _dbContext.CommentReactions
            .Where(cr => cr.CommentId == commentId && cr.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task CreateCommentReactionAsync(CommentReaction reaction)
    {
        await _dbContext.CommentReactions.AddAsync(reaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCommentReactionAsync(CommentReaction reaction)
    {
        _dbContext.CommentReactions.Update(reaction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCommentReactionAsync(CommentReaction reaction)
    {
        _dbContext.CommentReactions.Remove(reaction);
        await _dbContext.SaveChangesAsync();
    }
}