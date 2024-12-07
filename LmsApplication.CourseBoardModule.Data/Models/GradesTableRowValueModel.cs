using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class GradesTableRowValueModel
{
    public required Guid Id { get; set; }
    
    public required string? TeacherComment { get; set; }
    
    public required UserExchangeModel Teacher { get; set; }
    
    public required object? Value { get; set; }
}

public class UserGradesTableRowValueModel
{
    public required Guid? Id { get; set; }
    
    public required string? TeacherComment { get; set; }
    
    public required UserExchangeModel? Teacher { get; set; }
    
    public required UserExchangeModel Student { get; set; }
    
    public required object? Value { get; set; }
}
