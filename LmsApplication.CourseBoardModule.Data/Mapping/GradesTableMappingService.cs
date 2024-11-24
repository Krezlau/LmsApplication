using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Models;

namespace LmsApplication.CourseBoardModule.Data.Mapping;

public static class GradesTableMappingService
{
    public static GradesTableRowDefinitionModel ToModel(this GradesTableRowDefinition entity)
    {
        return new GradesTableRowDefinitionModel
        {
            Id = entity.Id,
            CourseEditionId = entity.CourseEditionId,
            Title = entity.Title,
            Description = entity.Description,
            Date = entity.Date,
            RowType = entity.RowType,
            IsSummed = entity.IsSummed
        };
    }
}