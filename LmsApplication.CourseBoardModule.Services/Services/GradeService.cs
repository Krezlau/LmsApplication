using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Mapping;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IGradeService
{
    Task<UserGradesModel> GetCurrentUserGradesAsync(Guid editionId);
    
    Task<UserGradesModel> GetUserGradesAsync(Guid editionId, string userId);
    
    Task<RowGradesModel> GetRowGradesAsync(Guid editionId, Guid rowId);
    
    Task<GradesTableRowValueModel> UpdateRowValueAsync(Guid editionId, Guid rowId, string userId, UpdateRowValueModel model);
    
    Task DeleteRowValueAsync(Guid editionId, Guid rowId, string userId);
    
    Task<FinalGradeModel> CreateFinalGradeAsync(Guid editionId, FinalGradeCreateModel model);
    
    Task<FinalGradeModel> UpdateFinalGradeAsync(Guid editionId, FinalGradeCreateModel model);
    
    Task DeleteFinalGradeAsync(Guid editionId, string userId);
}

public class GradeService : CourseBoardService, IGradeService
{
    private readonly IGradesTableRowValueRepository _gradesTableRowValueRepository;
    private readonly IGradesTableRowDefinitionRepository _gradesTableRowDefinitionRepository;
    private readonly IValidationService<UpdateRowValueValidationModel> _updateRowValueModelValidationService;
    private readonly IUserProvider _userProvider;
    private readonly IFinalGradeRepository _finalGradeRepository;
    private readonly IValidationService<UpdateFinalGradeValidationModel> _updateFinalGradeValidationService;
    private readonly IValidationService<CreateFinalGradeValidationModel> _createFinalGradeValidationService;

    public GradeService(
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext,
        IGradesTableRowValueRepository gradesTableRowValueRepository,
        IGradesTableRowDefinitionRepository gradesTableRowDefinitionRepository,
        IUserProvider userProvider,
        IValidationService<UpdateRowValueValidationModel> updateRowValueModelValidationService,
        IFinalGradeRepository finalGradeRepository,
        IValidationService<UpdateFinalGradeValidationModel> updateFinalGradeValidationService,
        IValidationService<CreateFinalGradeValidationModel> createFinalGradeValidationService) : base(courseEditionProvider, userContext)
    {
        _gradesTableRowValueRepository = gradesTableRowValueRepository;
        _gradesTableRowDefinitionRepository = gradesTableRowDefinitionRepository;
        _userProvider = userProvider;
        _updateRowValueModelValidationService = updateRowValueModelValidationService;
        _finalGradeRepository = finalGradeRepository;
        _updateFinalGradeValidationService = updateFinalGradeValidationService;
        _createFinalGradeValidationService = createFinalGradeValidationService;
    }

    public async Task<UserGradesModel> GetCurrentUserGradesAsync(Guid editionId)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        
        return await GetUserGradesFromDbAsync(editionId, userId);
    }

    public async Task<UserGradesModel> GetUserGradesAsync(Guid editionId, string userId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        return await GetUserGradesFromDbAsync(editionId, userId);
    }

    public async Task<RowGradesModel> GetRowGradesAsync(Guid editionId, Guid rowId)
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
        
        
        return new RowGradesModel
        {
            Row = row.ToModel(),
            Values = grades
        };
    }

    public async Task<GradesTableRowValueModel> UpdateRowValueAsync(Guid editionId, Guid rowId, string userId, UpdateRowValueModel model)
    {
        var teacherId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, teacherId);
        
        var validationModel = new UpdateRowValueValidationModel
        {
            StudentId = userId,
            CourseEditionId = editionId,
            Value = model.Value,
            TeacherComment = model.TeacherComment,
            RowDefinition = await _gradesTableRowDefinitionRepository.GetByIdAsync(rowId),
            Teacher = await _userProvider.GetUserByIdAsync(teacherId),
        };
        await _updateRowValueModelValidationService.ValidateAndThrowAsync(validationModel);
        
        var grade = await _gradesTableRowValueRepository.GetGradesTableRowValueAsync(rowId, userId);
        if (grade is null)
        {
            grade = CreateNewGrade(validationModel.RowDefinition!, rowId, userId, teacherId, model);
            
            await _gradesTableRowValueRepository.AddAsync(grade);
        }
        else
        {
            UpdateGrade(grade, model);
            
            await _gradesTableRowValueRepository.UpdateAsync(grade);
        }
        
        return grade.ToModel(validationModel.Teacher!, validationModel.RowDefinition!.RowType);
    }

    public async Task DeleteRowValueAsync(Guid editionId, Guid rowId, string userId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        var grade = await _gradesTableRowValueRepository.GetGradesTableRowValueAsync(rowId, userId);
        if (grade is null)
            throw new KeyNotFoundException("Grade not found.");
        
        await _gradesTableRowValueRepository.DeleteAsync(grade);
    }

    public async Task<FinalGradeModel> CreateFinalGradeAsync(Guid editionId, FinalGradeCreateModel model)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);

        var validationModel = new CreateFinalGradeValidationModel
        {
            CourseEditionId = editionId,
            Value = model.Value,
            Teacher = await _userProvider.GetUserByIdAsync(userId),
            StudentId = model.UserId,
        };
        await _createFinalGradeValidationService.ValidateAndThrowAsync(validationModel);

        var finalGrade = new FinalGrade
        {
            UserId = model.UserId,
            Value = model.Value,
            TeacherId = userId,
            CourseEditionId = editionId,
        };
        
        await _finalGradeRepository.CreateAsync(finalGrade);

        return finalGrade.ToModel(validationModel.Teacher!);
    }

    public async Task<FinalGradeModel> UpdateFinalGradeAsync(Guid editionId, FinalGradeCreateModel model)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);

        var validationModel = new UpdateFinalGradeValidationModel
        {
            FinalGrade = await _finalGradeRepository.GetFinalGradeAsync(editionId, model.UserId),
            CourseEditionId = editionId,
            Value = model.Value,
            Teacher = await _userProvider.GetUserByIdAsync(userId),
            StudentId = model.UserId,
        };
        await _updateFinalGradeValidationService.ValidateAndThrowAsync(validationModel);

        validationModel.FinalGrade!.TeacherId = userId;
        validationModel.FinalGrade!.Value = model.Value;
        await _finalGradeRepository.UpdateAsync(validationModel.FinalGrade);

        return validationModel.FinalGrade.ToModel(validationModel.Teacher!);
    }

    public async Task DeleteFinalGradeAsync(Guid editionId, string userId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        var finalGrade = await _finalGradeRepository.GetFinalGradeAsync(editionId, userId);
        if (finalGrade is null)
            throw new KeyNotFoundException("Final grade not found.");
        
        await _finalGradeRepository.DeleteAsync(finalGrade);
    }

    private async Task<UserGradesModel> GetUserGradesFromDbAsync(Guid editionId, string userId)
    {
        var rows = await _gradesTableRowDefinitionRepository.GetGradesTableForUserAsync(editionId, userId);
        var finalGrade = await _finalGradeRepository.GetFinalGradeAsync(editionId, userId);

        var teachersToGet = rows.SelectMany(x => x.Values.Select(v => v.TeacherId)).ToList();
        if (finalGrade is not null) teachersToGet.Add(finalGrade.TeacherId);
        
        var teachers = await _userProvider.GetUsersByIdsAsync(teachersToGet.Distinct().ToList());

        var grades = rows.Select(row =>
        {
            var value = row.Values.FirstOrDefault();
            return new GradeModel
            {
                Row = row.ToModel(),
                Value = value?.ToModel(teachers[value.TeacherId], row.RowType) ?? null,
            };
        }).ToList();

        return new UserGradesModel
        {
            Grades = grades,
            FinalGrade = finalGrade?.ToModel(teachers[finalGrade.TeacherId]),
        };
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