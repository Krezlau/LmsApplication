using FluentValidation;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Mapping;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IGradeService
{
    Task<List<GradeModel>> GetCurrentUserGradesAsync(Guid editionId);
    
    Task<List<GradeModel>> GetUserGradesAsync(Guid editionId, string userId);
    
    Task<UserGradesModel> GetRowGradesAsync(Guid editionId, Guid rowId);
    
    Task UpdateRowValueAsync(Guid editionId, Guid rowId, string userId, UpdateRowValueModel model);
    
    Task DeleteRowValueAsync(Guid editionId, Guid rowId, string userId);
}

public class GradeService : CourseBoardService, IGradeService
{
    private readonly IGradesTableRowValueRepository _gradesTableRowValueRepository;
    private readonly IGradesTableRowDefinitionRepository _gradesTableRowDefinitionRepository;
    private readonly IValidationService<UpdateRowValueModel> _updateRowValueModelValidationService;
    private readonly IUserProvider _userProvider;

    public GradeService(
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext,
        IGradesTableRowValueRepository gradesTableRowValueRepository,
        IGradesTableRowDefinitionRepository gradesTableRowDefinitionRepository,
        IUserProvider userProvider,
        IValidationService<UpdateRowValueModel> updateRowValueModelValidationService) : base(courseEditionProvider, userContext)
    {
        _gradesTableRowValueRepository = gradesTableRowValueRepository;
        _gradesTableRowDefinitionRepository = gradesTableRowDefinitionRepository;
        _userProvider = userProvider;
        _updateRowValueModelValidationService = updateRowValueModelValidationService;
    }

    public async Task<List<GradeModel>> GetCurrentUserGradesAsync(Guid editionId)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        return await GetUserGradesFromDbAsync(editionId, userId);
    }

    public async Task<List<GradeModel>> GetUserGradesAsync(Guid editionId, string userId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        return await GetUserGradesFromDbAsync(editionId, userId);
    }

    public async Task<UserGradesModel> GetRowGradesAsync(Guid editionId, Guid rowId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        var row = await _gradesTableRowDefinitionRepository.GetRowWithValuesAsync(rowId);
        if (row is null) 
            throw new KeyNotFoundException("Row not found.");
        
        var students = await CourseEditionProvider.GetCourseEditionStudentsAsync(editionId);
        var userIds = row.Values.Select(x => x.UserId).ToList()
            .Concat(row.Values.Select(x => x.TeacherId).ToList())
            .Concat(students)
            .Distinct()
            .ToList();
        var users = await _userProvider.GetUsersByIdsAsync(userIds);
        
        var grades = new List<UserGradesTableRowValueModel>();
        foreach (var student in students)
        {
            var grade = row.Values.FirstOrDefault(x => x.UserId == student);
            if (grade is null)
            {
                grades.Add(new UserGradesTableRowValueModel
                {
                    Id = null,
                    Student = users[student],
                    Teacher = null,
                    Value = null,
                    TeacherComment = null,
                });
            }

            if (grade is not null)
            {
                grades.Add(grade.ToUserModel(users[grade.TeacherId], users[student], row.RowType));
            }
        }
        
        
        return new UserGradesModel
        {
            Row = row.ToModel(),
            Values = grades
        };
    }

    public async Task UpdateRowValueAsync(Guid editionId, Guid rowId, string userId, UpdateRowValueModel model)
    {
        var teacherId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, teacherId);

        var rowDefinition = await _gradesTableRowDefinitionRepository.GetByIdAsync(rowId);
        if (rowDefinition is null) 
            throw new KeyNotFoundException("Row not found.");
        
        var grade = await _gradesTableRowValueRepository.GetGradesTableRowValueAsync(rowId, userId);
        var context = new ValidationContext<UpdateRowValueModel>(model)
        {
            RootContextData =
            {
                {nameof(GradesTableRowDefinition), rowDefinition}
            }
        };
        await _updateRowValueModelValidationService.ValidateAndThrowAsync(context);
        
        if (grade is null)
        {
            grade = CreateNewGrade(rowDefinition, rowId, userId, teacherId, model);
            
            await _gradesTableRowValueRepository.AddAsync(grade);
        }
        else
        {
            UpdateGrade(grade, model);
            
            await _gradesTableRowValueRepository.UpdateAsync(grade);
        }
    }

    public async Task DeleteRowValueAsync(Guid editionId, Guid rowId, string userId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        var grade = await _gradesTableRowValueRepository.GetGradesTableRowValueAsync(rowId, userId);
        if (grade is null)
            throw new KeyNotFoundException("Grade not found.");
        
        await _gradesTableRowValueRepository.DeleteAsync(grade);
    }

    private async Task<List<GradeModel>> GetUserGradesFromDbAsync(Guid editionId, string userId)
    {
        var rows = await _gradesTableRowDefinitionRepository.GetGradesTableForUserAsync(editionId, userId);
        
        var teachers = await _userProvider.GetUsersByIdsAsync(
            rows.SelectMany(x => x.Values.Select(v => v.TeacherId)).Distinct().ToList());

        return rows.Select(row =>
        {
            var value = row.Values.FirstOrDefault();
            return new GradeModel
            {
                Row = row.ToModel(),
                Value = value?.ToModel(teachers[value.TeacherId], row.RowType) ?? null,
            };
        }).ToList();
    }
    
    private static GradesTableRowValue CreateNewGrade(GradesTableRowDefinition rowDefinition, Guid rowId, string userId,
        string teacherId, UpdateRowValueModel model)
    {
        return rowDefinition.RowType switch
        {
            RowType.Text => new GradesTableRowTextValue
            {
                UserId = userId,
                RowDefinitionId = rowId,
                RowType = RowType.Text,
                TeacherId = teacherId,
                TeacherComment = model.TeacherComment ?? "",
                Value = model.Value
            },
            RowType.Number => new GradesTableRowNumberValue
            {
                UserId = userId,
                RowDefinitionId = rowId,
                RowType = RowType.Number,
                TeacherId = teacherId,
                TeacherComment = model.TeacherComment ?? "",
                Value = decimal.Parse(model.Value)
            },
            RowType.Bool => new GradesTableRowBoolValue
            {
                UserId = userId,
                RowDefinitionId = rowId,
                RowType = RowType.Bool,
                TeacherId = teacherId,
                TeacherComment = model.TeacherComment ?? "",
                Value = bool.Parse(model.Value)
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private static void UpdateGrade(GradesTableRowValue grade, UpdateRowValueModel model)
    {
        grade.TeacherComment = model.TeacherComment ?? "";
        
        switch (grade.RowType)
        {
            case RowType.Text:
                ((GradesTableRowTextValue)grade).Value = model.Value;
                break;
            case RowType.Number:
                ((GradesTableRowNumberValue)grade).Value = decimal.Parse(model.Value);
                break;
            case RowType.Bool:
                ((GradesTableRowBoolValue)grade).Value = bool.Parse(model.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}