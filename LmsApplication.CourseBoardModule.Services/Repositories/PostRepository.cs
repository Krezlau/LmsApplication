using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseBoardModule.Services.Repositories;

public interface IPostRepository
{
    Task<(int totalCount, List<Post> posts)> GetPostsAsync(Guid editionId, int page, int pageSize);
    
    Task CreatePostAsync(Post post);
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
            .OrderByDescending(x => x.CreatedAtUtc);
        
        var totalCount = await query.CountAsync();
        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, posts);
    }

    public async Task CreatePostAsync(Post post)
    {
        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();
    }
}