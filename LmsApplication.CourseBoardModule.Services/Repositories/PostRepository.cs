using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseBoardModule.Services.Repositories;

public interface IPostRepository
{
    Task<(int totalCount, List<Post> posts)> GetPostsAsync(Guid editionId, int page, int pageSize);
    
    Task<Post?> GetPostByIdAsync(Guid postId);
    
    Task CreatePostAsync(Post post);
    
    Task UpdatePostAsync(Post post);
    
    Task DeletePostAsync(Post post);
}

public class PostRepository : IPostRepository
{
    private readonly CourseBoardDbContext _dbContext;

    public PostRepository(CourseBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(int totalCount, List<Post> posts)> GetPostsAsync(Guid editionId, int page, int pageSize)
    {
        var query = _dbContext.Posts
            .Where(x => x.EditionId == editionId)
            .Include(x => x.Reactions)
            .Include(x => x.Comments)
            .OrderByDescending(x => x.CreatedAtUtc);
        
        var totalCount = await query.CountAsync();
        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, posts);
    }

    public async Task<Post?> GetPostByIdAsync(Guid postId)
    {
        return await _dbContext.Posts
            .FirstOrDefaultAsync(x => x.Id == postId);
    }

    public async Task CreatePostAsync(Post post)
    {
        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Post post)
    {
        _dbContext.Posts.Update(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePostAsync(Post post)
    {
        post.IsDeleted = true;
        post.DeletedAtUtc = DateTime.UtcNow;
        _dbContext.Posts.Update(post);
        await _dbContext.SaveChangesAsync();
    }
}