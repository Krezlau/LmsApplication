using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseBoardModule.Services.Repositories;

public interface ICommentRepository
{
    Task<(int totalCount, List<Comment> comments)> GetCommentsForPostAsync(Guid postId, int page, int pageSize);
    
    Task<Comment?> GetCommentByIdAsync(Guid commentId);
    
    Task CreateCommentAsync(Comment comment);
    
    Task UpdateCommentAsync(Comment comment);
    
    Task DeleteCommentAsync(Comment comment);
}

public class CommentRepository : ICommentRepository
{
    private readonly CourseBoardDbContext _dbContext;

    public CommentRepository(CourseBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int totalCount, List<Comment> comments)> GetCommentsForPostAsync(Guid postId, int page, int pageSize)
    {
        var query = _dbContext.Comments
            .Where(x => x.PostId == postId)
            .Include(x => x.Reactions)
            .OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync();
        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, comments);
    }

    public async Task<Comment?> GetCommentByIdAsync(Guid commentId)
    {
        return await _dbContext.Comments
            .Include(x => x.Reactions)
            .FirstOrDefaultAsync(x => x.Id == commentId);
    }

    public async Task CreateCommentAsync(Comment comment)
    {
        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        _dbContext.Comments.Update(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(Comment comment)
    {
        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
}