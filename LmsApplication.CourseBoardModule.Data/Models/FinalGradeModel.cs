using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Data.Models;

public class FinalGradeModel
{
    public required Guid Id { get; set; }

    public required decimal Value { get; set; }

    public required UserExchangeModel Teacher { get; set; }
}