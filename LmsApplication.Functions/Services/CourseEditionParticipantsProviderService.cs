using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Services.Repositories;
using LmsApplication.UserModule.Services.Repositories;

namespace LmsApplication.Functions.Services;

public interface ICourseEditionParticipantsProviderService
{
    Task<List<UserExchangeModel>> GetCourseEditionParticipantsAsync(Guid courseEditionId);
}

public class CourseEditionParticipantsProviderService : ICourseEditionParticipantsProviderService
{
    private readonly ICourseEditionRepository _courseEditionRepository;
    private readonly IUserRepository _userRepository;

    public CourseEditionParticipantsProviderService(
        ICourseEditionRepository courseEditionRepository,
        IUserRepository userRepository)
    {
        _courseEditionRepository = courseEditionRepository;
        _userRepository = userRepository;
    }

    public async Task<List<UserExchangeModel>> GetCourseEditionParticipantsAsync(Guid courseEditionId)
    {
        var courseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(courseEditionId);
        if (courseEdition is null)
            return [];

        var studentIds = courseEdition.Participants.Where(x => x.ParticipantRole == UserRole.Student)
            .Select(x => x.ParticipantId)
            .ToList();
        var participants = await _userRepository.GetUsersByIdsAsync(studentIds);
        
        return participants.Select(x => new UserExchangeModel
        {
            Id = x.Id,
            Email = x.Email!,
            Name = x.Name,
            Surname = x.Surname,
            Role = UserRole.Student
        }).ToList();
    }
}