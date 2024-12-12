using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models.Validation;

public class UpdateRowDefinitionValidationModel
{
    public required string Title { get; set; } = string.Empty;
    
    public required string? Description { get; set; }
    
    public required DateTime? Date { get; set; }
    
    public required bool IsSummed { get; set; }
    
    public required GradesTableRowDefinition? RowDefinition { get; set; }
}