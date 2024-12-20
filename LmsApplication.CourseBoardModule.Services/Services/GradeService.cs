using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Mapping;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;
using RowType = LmsApplication.CourseBoardModule.Data.Entities.RowType;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IGradeService
{
    Task<UserGradesModel> GetCurrentUserGradesAsync(Guid editionId);
    
    Task<UserGradesModel> GetUserGradesAsync(Guid editionId, string userId);
    
    Task<RowGradesModel> GetRowGradesAsync(Guid editionId, Guid rowId);
    
    Task<GradesTableRowValueModel> UpdateRowValueAsync(Guid editionId, Guid rowId, string userId, UpdateRowValueModel model);
    
    Task DeleteRowValueAsync(Guid editionId, Guid rowId, string userId);
    
    Task<FinalGradeModel> CreateFinalGradeAsync(Guid editionId, FinalGradeCreateModel model);
    
    Task DeleteFinalGradeAsync(Guid editionId, string userId);
}

public class GradeService : CourseBoardService, IGradeService
{
    private readonly IGradesTableRowValueRepository _gradesTableRowValueRepository;
    private readonly IGradesTableRowDefinitionRepository _gradesTableRowDefinitionRepository;
    private readonly IValidationService<UpdateRowValueValidationModel> _updateRowValueModelValidationService;
    private readonly IUserProvider _userProvider;
    private readonly IFinalGradeRepository _finalGradeRepository;
    private readonly IValidationService<CreateFinalGradeValidationModel> _createFinalGradeValidationService;
    private readonly IQueueClient<GradeNotificationQueueMessage> _gradeNotificationQueueClient;
    private readonly IQueueClient<FinalGradeNotificationQueueMessage> _finalGradeNotificationQueueClient;

    public GradeService(
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext,
        IGradesTableRowValueRepository gradesTableRowValueRepository,
        IGradesTableRowDefinitionRepository gradesTableRowDefinitionRepository,
        IUserProvider userProvider,
        IValidationService<UpdateRowValueValidationModel> updateRowValueModelValidationService,
        IFinalGradeRepository finalGradeRepository,
        IValidationService<CreateFinalGradeValidationModel> createFinalGradeValidationService,
        IQueueClient<GradeNotificationQueueMessage> gradeNotificationQueueClient,
        IQueueClient<FinalGradeNotificationQueueMessage> finalGradeNotificationQueueClient) : base(courseEditionProvider, userContext)
    {
        _gradesTableRowValueRepository = gradesTableRowValueRepository;
        _gradesTableRowDefinitionRepository = gradesTableRowDefinitionRepository;
        _userProvider = userProvider;
        _updateRowValueModelValidationService = updateRowValueModelValidationService;
        _finalGradeRepository = finalGradeRepository;
        _createFinalGradeValidationService = createFinalGradeValidationService;
        _gradeNotificationQueueClient = gradeNotificationQueueClient;
        _finalGradeNotificationQueueClient = finalGradeNotificationQueueClient;
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
        await ValidateWriteAccessToEditionAsync(editionId);
        
        var validationModel = new UpdateRowValueValidationModel
        {
            Student = await _userProvider.GetUserByIdAsync(userId),
            CourseEdition = await CourseEditionProvider.GetCourseEditionAsync(editionId),
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
        
        await SendGradeNotificationAsync(validationModel, model);
        
        return grade.ToModel(validationModel.Teacher!, validationModel.RowDefinition!.RowType);
    }

    public async Task DeleteRowValueAsync(Guid editionId, Guid rowId, string userId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());
        await ValidateWriteAccessToEditionAsync(editionId);

        var grade = await _gradesTableRowValueRepository.GetGradesTableRowValueAsync(rowId, userId);
        if (grade is null)
            throw new KeyNotFoundException("Grade not found.");
        
        await _gradesTableRowValueRepository.DeleteAsync(grade);
    }

    public async Task<FinalGradeModel> CreateFinalGradeAsync(Guid editionId, FinalGradeCreateModel model)
    {
        var userId = UserContext.GetUserId();
        await ValidateUserAccessToEditionAsync(editionId, userId);
        await ValidateWriteAccessToEditionAsync(editionId);

        var validationModel = new CreateFinalGradeValidationModel
        {
            CourseEdition = await CourseEditionProvider.GetCourseEditionAsync(editionId),
            Value = model.Value,
            Teacher = await _userProvider.GetUserByIdAsync(userId),
            Student = await _userProvider.GetUserByIdAsync(model.UserId),
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

        await _finalGradeNotificationQueueClient.EnqueueAsync(new FinalGradeNotificationQueueMessage
        {
            User = validationModel.Student!,
            CourseEditionId = editionId,
            Grade = model.Value,
            Teacher = validationModel.Teacher!,
            TimeStampUtc = DateTime.UtcNow,
            CourseEditionName = validationModel.CourseEdition!.Name,
        });
        
        return finalGrade.ToModel(validationModel.Teacher!);
    }

    public async Task DeleteFinalGradeAsync(Guid editionId, string userId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());
        await ValidateWriteAccessToEditionAsync(editionId);

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
    
    private async Task SendGradeNotificationAsync(UpdateRowValueValidationModel validationModel, UpdateRowValueModel model)
    {
        var rowType = validationModel.RowDefinition!.RowType switch
        {
            RowType.Text => Core.Shared.QueueMessages.RowType.Text,
            RowType.Number => Core.Shared.QueueMessages.RowType.Number,
            RowType.Bool => Core.Shared.QueueMessages.RowType.Bool,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var message = new GradeNotificationQueueMessage
        {
            User = validationModel.Student!,
            CourseEditionId = validationModel.CourseEdition!.Id,
            Grade = model.Value,
            RowName = validationModel.RowDefinition!.Title,
            RowType = rowType,
            Teacher = validationModel.Teacher!,
            TeacherComment = model.TeacherComment,
            TimeStampUtc = DateTime.UtcNow,
            CourseEditionName = validationModel.CourseEdition!.Name,
        };
        
        await _gradeNotificationQueueClient.EnqueueAsync(message);
    }
}