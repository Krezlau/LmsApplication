using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class ReactionModel
{
    public required string Username { get; set; }
    
    public required ReactionType ReactionType { get; set; }
}