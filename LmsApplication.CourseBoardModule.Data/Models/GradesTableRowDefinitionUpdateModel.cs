namespace LmsApplication.CourseBoardModule.Data.Models;

public class GradesTableRowDefinitionUpdateModel
{
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime? Date { get; set; }
    
    public bool IsSummed { get; set; }
}