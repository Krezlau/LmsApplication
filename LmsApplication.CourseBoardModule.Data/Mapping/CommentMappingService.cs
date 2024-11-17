using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Data.Mapping;

public static class CommentMappingService
{
    public static CommentModel ToModel(
        this Comment comment,
        UserExchangeModel author,
        Dictionary<string, string> userNames,
        string currentUserId)
    {
        var currentUserReaction = comment.Reactions.FirstOrDefault(x => x.UserId == currentUserId)?.ReactionType ?? null;
        
        return new CommentModel()
        {
            Id = comment.Id,
            PostId = comment.PostId,
            Author = author,
            Content = comment.Content,
            CreatedAt = comment.CreatedAtUtc,
            UpdatedAt = comment.UpdatedAtUtc,
            Reactions = comment.Reactions.ToModel(),
            CurrentUserReaction = currentUserReaction, 
        };
    }
}