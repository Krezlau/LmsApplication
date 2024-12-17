using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Services.Repositories;
using LmsApplication.CourseModule.Services.Providers;

namespace LmsApplication.Core.Api.Providers;

public class CourseBoardProvider : ICourseBoardProvider
{
    private readonly IFinalGradeRepository _finalGradeRepository;

    public CourseBoardProvider(IFinalGradeRepository finalGradeRepository)
    {
        _finalGradeRepository = finalGradeRepository;
    }

    public async Task<Dictionary<Guid, FinalGradeModel>> GetFinalGradesDictionaryAsync(string userId)
    {
        var finalGrades = await _finalGradeRepository.GetUserFinalGradesAsync(userId);

        return finalGrades.ToDictionary(x => x.CourseEditionId, x => new FinalGradeModel
        {
            Id = x.Id,
            Value = x.Value
        });
    }
}