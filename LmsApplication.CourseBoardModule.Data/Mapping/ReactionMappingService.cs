using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Data.Mapping;

public static class ReactionMappingService
{
    public static ReactionModel ToModel(this List<PostReaction> reactions)
    {
        return new ReactionModel
        {
            SumOfReactions = reactions.Count,
            SumOfReactionsByType = reactions.GroupBy(x => x.ReactionType)
                .ToDictionary(x => x.Key, x => x.Count()),
        };
    }
    
    public static ReactionModel ToModel(this List<CommentReaction> reactions)
    {
        return new ReactionModel
        {
            SumOfReactions = reactions.Count,
            SumOfReactionsByType = reactions.GroupBy(x => x.ReactionType)
                .ToDictionary(x => x.Key, x => x.Count()),
        };
    }
}