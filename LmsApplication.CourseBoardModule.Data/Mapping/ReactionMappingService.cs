using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Data.Mapping;

public static class ReactionMappingService
{
    public static ReactionModel ToModel(this PostReaction reaction, string username)
    {
        return new ReactionModel
        {
            Username = username,
            ReactionType = reaction.ReactionType,
        };
    }
}