using LmsApplication.Core.Shared.Services;
using LmsApplication.CourseBoardModule.Data.Models;
using LmsApplication.CourseBoardModule.Services.Providers;
using LmsApplication.CourseBoardModule.Services.Repositories;

namespace LmsApplication.CourseBoardModule.Services.Services;

public interface IGradeService
{
    Task<List<GradeModel>> GetCurrentUserGradesAsync(Guid editionId);
}

public class GradeService : CourseBoardService, IGradeService
{
    private readonly IGradesTableRowValueRepository _gradesTableRowValueRepository;
    private readonly IGradesTableRowDefinitionRepository _gradesTableRowDefinitionRepository;

    public GradeService(
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext,
        IGradesTableRowValueRepository gradesTableRowValueRepository,
        IGradesTableRowDefinitionRepository gradesTableRowDefinitionRepository) : base(courseEditionProvider, userContext)
    {
        _gradesTableRowValueRepository = gradesTableRowValueRepository;
        _gradesTableRowDefinitionRepository = gradesTableRowDefinitionRepository;
    }

    public async Task<List<GradeModel>> GetCurrentUserGradesAsync(Guid editionId)
    {
        await ValidateUserAccessToEditionAsync(editionId, UserContext.GetUserId());
    }
}