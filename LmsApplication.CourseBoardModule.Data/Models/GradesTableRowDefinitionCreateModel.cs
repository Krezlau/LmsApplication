using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class GradesTableRowDefinitionCreateModel
{
    public Guid CourseEditionId { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime? Date { get; set; }
    
    public RowType RowType { get; set; }
    
    public bool IsSummed { get; set; }
}