using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Data.Mapping;

public static class PostMappingService
{
    public static PostModel ToModel(
        this Post post,
        UserExchangeModel author,
        Dictionary<string, string> userNames,
        string currentUserId)
    {
        var currentUserReaction = post.Reactions.FirstOrDefault(x => x.UserId == currentUserId)?.ReactionType ?? null;
        return new PostModel
        {
            Id = post.Id,
            Content = post.Content,
            EditionId = post.EditionId,
            Author = author,
            Reactions = post.Reactions.ToModel(),
            CurrentUserReaction = currentUserReaction,
            CommentsCount = post.Comments.Count,
            CreatedAt = post.CreatedAtUtc,
            UpdatedAt = post.UpdatedAtUtc,
        };
    }
}