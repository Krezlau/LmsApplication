using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class CommentModel
{
    public required Guid Id { get; set; }
    
    public required string Content { get; set; }
    
    public required Guid PostId { get; set; }
    
    public required UserExchangeModel Author { get; set; }
    
    public required List<ReactionModel> Reactions { get; set; }
    
    public required ReactionModel? CurrentUserReaction { get; set; }
    
    public required DateTime CreatedAt { get; set; }
    
    public required DateTime? UpdatedAt { get; set; }
}