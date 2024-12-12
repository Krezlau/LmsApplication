using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseBoardModule.Data.Mapping;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Data.Models.Validation;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IGradesTableRowDefinitionService
{
    Task<List<GradesTableRowDefinitionModel>> GetGradesTableRowDefinitionsAsync(Guid editionId);
    
    Task<GradesTableRowDefinitionModel> CreateGradesTableRowDefinitionAsync(Guid editionId, GradesTableRowDefinitionCreateModel model);
    
    Task<GradesTableRowDefinitionModel> UpdateGradesTableRowDefinitionAsync(Guid editionId, Guid rowId, GradesTableRowDefinitionUpdateModel model);
    
    Task DeleteGradesTableRowDefinitionAsync(Guid editionId, Guid rowId);
}

public class GradesTableRowDefinitionService : CourseBoardService, IGradesTableRowDefinitionService
{
    private readonly IGradesTableRowDefinitionRepository _gradesTableRowDefinitionRepository;
    private readonly IValidationService<GradesTableRowDefinitionCreateModel> _createValidationService;
    private readonly IValidationService<UpdateRowDefinitionValidationModel> _updateValidationService;

    public GradesTableRowDefinitionService(
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext,
        IGradesTableRowDefinitionRepository gradesTableRowDefinitionRepository,
        IValidationService<GradesTableRowDefinitionCreateModel> createValidationService,
        IValidationService<UpdateRowDefinitionValidationModel> updateValidationService) : base(courseEditionProvider, userContext)
    {
        _gradesTableRowDefinitionRepository = gradesTableRowDefinitionRepository;
        _createValidationService = createValidationService;
        _updateValidationService = updateValidationService;
    }

    public async Task<List<GradesTableRowDefinitionModel>> GetGradesTableRowDefinitionsAsync(Guid editionId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        var rows = await _gradesTableRowDefinitionRepository.GetGradesTableRowDefinitionsAsync(editionId);
        
        return rows.Select(x => x.ToModel()).ToList();
    }

    public async Task<GradesTableRowDefinitionModel> CreateGradesTableRowDefinitionAsync(Guid editionId, GradesTableRowDefinitionCreateModel model)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        await _createValidationService.ValidateAndThrowAsync(model);
        
        var entity = new GradesTableRowDefinition
        {
            Title = model.Title,
            Description = model.Description,
            Date = model.Date,
            RowType = model.RowType,
            IsSummed = model.IsSummed, 
            CourseEditionId = model.CourseEditionId,
        };
        
        await _gradesTableRowDefinitionRepository.CreateAsync(entity);
        
        return entity.ToModel();
    }

    public async Task<GradesTableRowDefinitionModel> UpdateGradesTableRowDefinitionAsync(Guid editionId, Guid rowId, GradesTableRowDefinitionUpdateModel model)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());

        var validationModel = new UpdateRowDefinitionValidationModel
        {
            Title = model.Title,
            Description = model.Description,
            Date = model.Date,
            IsSummed = model.IsSummed,
            RowDefinition = await _gradesTableRowDefinitionRepository.GetByIdAsync(rowId),
        };
        
        await _updateValidationService.ValidateAndThrowAsync(validationModel);
        
        validationModel.RowDefinition!.Title = model.Title;
        validationModel.RowDefinition!.Description = model.Description;
        validationModel.RowDefinition!.Date = model.Date;
        validationModel.RowDefinition!.IsSummed = model.IsSummed;
        
        await _gradesTableRowDefinitionRepository.UpdateAsync(validationModel.RowDefinition!);
        
        return validationModel.RowDefinition!.ToModel();
    }

    public async Task DeleteGradesTableRowDefinitionAsync(Guid editionId, Guid rowId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());
        
        var entity = await _gradesTableRowDefinitionRepository.GetByIdAsync(rowId);
        if (entity is null) 
            throw new KeyNotFoundException("Row not found.");
        
        await _gradesTableRowDefinitionRepository.DeleteAsync(entity);
    }
}