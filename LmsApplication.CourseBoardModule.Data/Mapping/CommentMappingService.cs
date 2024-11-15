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
        return new CommentModel()
        {
            Id = comment.Id,
            PostId = comment.PostId,
            Author = author,
            Content = comment.Content,
            CreatedAt = comment.CreatedAtUtc,
            UpdatedAt = comment.UpdatedAtUtc,
            Reactions = comment.Reactions.Select(x => x.ToModel(userNames[x.UserId])).ToList(),
            CurrentUserReaction = comment.Reactions.FirstOrDefault(x => x.UserId == currentUserId)?.ToModel(userNames[currentUserId]),
        };
    }
}