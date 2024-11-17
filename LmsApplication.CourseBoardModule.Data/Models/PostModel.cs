using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class PostModel
{
    public required Guid Id { get; set; }
    
    public required string Content { get; set; }
    
    public required Guid EditionId { get; set; }
    
    public required UserExchangeModel Author { get; set; }
    
    public required ReactionModel Reactions { get; set; }
    
    public required ReactionType? CurrentUserReaction { get; set; }
    
    public required int CommentsCount { get; set; }
    
    public required DateTime CreatedAt { get; set; }
    
    public required DateTime? UpdatedAt { get; set; }
}