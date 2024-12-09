using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class GradeModel
{
    public required GradesTableRowDefinitionModel Row { get; set; }
    
    public required GradesTableRowValueModel? Value { get; set; }
}

public class RowGradesModel
{
    public required GradesTableRowDefinitionModel Row { get; set; }
    
    public required List<UserGradesTableRowValueModel> Values { get; set; }
}

public class UserGradesModel
{
    public required List<GradeModel> Grades { get; set; }
    
    public required FinalGradeModel? FinalGrade { get; set; }
}