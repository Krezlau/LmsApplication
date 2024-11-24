using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class GradesTableRowDefinitionModel
{
    public required Guid Id { get; set; }
    
    public required Guid CourseEditionId { get; set; }
    
    public required string Title { get; set; }
    
    public required string? Description { get; set; }
    
    public required DateTime? Date { get; set; }
    
    public required RowType RowType { get; set; }
    
    public required bool IsSummed { get; set; }
}