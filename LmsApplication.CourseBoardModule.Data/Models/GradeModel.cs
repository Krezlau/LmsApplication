namespace LmsApplication.CourseBoardModule.Data.Models;

public class GradeModel
{
    public required GradesTableRowDefinitionModel Row { get; set; }
    
    public required GradesTableRowValueModel? Value { get; set; }
}

public class UserGradesModel
{
    public required GradesTableRowDefinitionModel Row { get; set; }
    
    public required List<UserGradesTableRowValueModel> Values { get; set; }
}
