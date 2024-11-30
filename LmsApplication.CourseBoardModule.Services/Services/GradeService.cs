using System.ComponentModel.DataAnnotations;
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

        var userIds = row.Values.Select(x => x.UserId).ToList()
            .Concat(row.Values.Select(x => x.TeacherId).ToList())
            .Distinct()
            .ToList();
        var users = await _userProvider.GetUsersByIdsAsync(userIds);
        
        return new UserGradesModel
        {
            Row = row.ToModel(),
            Values = row.Values.Select(value => value.ToUserModel(users[value.TeacherId], users[value.UserId], row.RowType)).ToList(),
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
                {nameof(GradesTableRowValue), rowDefinition}
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
                TeacherComment = model.TeacherComment,
                Value = (string)model.Value
            },
            RowType.Number => new GradesTableRowNumberValue
            {
                UserId = userId,
                RowDefinitionId = rowId,
                RowType = RowType.Number,
                TeacherId = teacherId,
                TeacherComment = model.TeacherComment,
                Value = (decimal)model.Value
            },
            RowType.Bool => new GradesTableRowBoolValue
            {
                UserId = userId,
                RowDefinitionId = rowId,
                RowType = RowType.Bool,
                TeacherId = teacherId,
                TeacherComment = model.TeacherComment,
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private static void UpdateGrade(GradesTableRowValue grade, UpdateRowValueModel model)
    {
        grade.TeacherComment = model.TeacherComment;
        
        switch (grade.RowType)
        {
            case RowType.Text:
                ((GradesTableRowTextValue)grade).Value = (string)model.Value;
                break;
            case RowType.Number:
                ((GradesTableRowNumberValue)grade).Value = (decimal)model.Value;
                break;
            case RowType.Bool:
                ((GradesTableRowBoolValue)grade).Value = (bool)model.Value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}