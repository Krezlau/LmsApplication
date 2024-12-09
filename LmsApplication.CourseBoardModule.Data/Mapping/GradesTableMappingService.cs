using LmsApplication.Core.Shared.Models;
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

    public static GradesTableRowValueModel ToModel(this GradesTableRowValue entity, UserExchangeModel teacher, RowType rowType)
    {
        object? value = rowType switch
        {
            RowType.None => null,
            RowType.Number => entity is GradesTableRowNumberValue numberValue ? numberValue.Value : null,
            RowType.Text => entity is GradesTableRowTextValue textValue ? textValue.Value : null,
            RowType.Bool => entity is GradesTableRowBoolValue boolValue ? boolValue.Value : null,
            _ => throw new ArgumentOutOfRangeException(nameof(rowType), rowType, null)
        };
        
        return new GradesTableRowValueModel
        {
            Id = entity.Id,
            Teacher = teacher,
            TeacherComment = entity.TeacherComment,
            Value = value
        };
    }
    
    public static UserGradesTableRowValueModel ToUserModel(this GradesTableRowValue entity, UserExchangeModel teacher, UserExchangeModel student, RowType rowType)
    {
        object? value = rowType switch
        {
            RowType.None => null,
            RowType.Number => entity is GradesTableRowNumberValue numberValue ? numberValue.Value : null,
            RowType.Text => entity is GradesTableRowTextValue textValue ? textValue.Value : null,
            RowType.Bool => entity is GradesTableRowBoolValue boolValue ? boolValue.Value : null,
            _ => throw new ArgumentOutOfRangeException(nameof(rowType), rowType, null)
        };
        
        return new UserGradesTableRowValueModel
        {
            Id = entity.Id,
            Teacher = teacher,
            TeacherComment = entity.TeacherComment,
            Value = value,
            Student = student
        };
    }
    
    public static FinalGradeModel ToModel(this FinalGrade entity, UserExchangeModel teacher)
    {
        return new FinalGradeModel
        {
            Id = entity.Id,
            Value = entity.Value,
            Teacher = teacher
        };
    }
}