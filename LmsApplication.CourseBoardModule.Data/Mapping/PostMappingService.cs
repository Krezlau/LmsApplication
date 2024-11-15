using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Data.Mapping;

public static class PostMappingService
{
    public static PostModel ToModel(this Post post,
        UserExchangeModel author,
        Dictionary<string, string> userNames,
        string currentUserId)
    {
        return new PostModel
        {
            Id = post.Id,
            Content = post.Content,
            Author = author,
            Reactions = post.Reactions.Select(x => x.ToModel(userNames[x.UserId])).ToList(),
            CurrentUserReaction = post.Reactions.FirstOrDefault(x => x.UserId == currentUserId)?.ToModel(userNames[currentUserId]),
            CommentsCount = post.Comments.Count,
            CreatedAt = post.CreatedAtUtc,
            UpdatedAt = post.UpdatedAtUtc,
        };
    }
}