namespace LmsApplication.CourseBoardModule.Data.Models;

public class GradeModel
{
    public required GradesTableRowDefinitionModel Row { get; set; }
    
    public required GradesTableRowValueModel Value { get; set; }
}