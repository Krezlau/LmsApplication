using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class ReactionModel
{
    public required int SumOfReactions { get; set; }
    
    public required Dictionary<ReactionType, int> SumOfReactionsByType { get; set; }
}