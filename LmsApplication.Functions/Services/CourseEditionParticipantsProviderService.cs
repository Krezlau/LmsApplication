using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseModule.Services.Repositories;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.Functions.Services;

public interface ICourseEditionParticipantsProviderService
{
    Task<List<UserExchangeModel>> GetCourseEditionParticipantsAsync(Guid courseEditionId);
}

public class CourseEditionParticipantsProviderService : ICourseEditionParticipantsProviderService
{
    private readonly ICourseEditionRepository _courseEditionRepository;
    private readonly UserManager<User> _userManager;

    public CourseEditionParticipantsProviderService(ICourseEditionRepository courseEditionRepository, UserManager<User> userManager)
    {
        _courseEditionRepository = courseEditionRepository;
        _userManager = userManager;
    }

    public async Task<List<UserExchangeModel>> GetCourseEditionParticipantsAsync(Guid courseEditionId)
    {
        var courseEdition = await _courseEditionRepository.GetCourseEditionByIdAsync(courseEditionId);
        if (courseEdition is null)
            return [];

        var studentIds = courseEdition.Participants.Where(x => x.ParticipantRole == UserRole.Student)
            .Select(x => x.ParticipantId)
            .ToList();
        var participants = await _userManager.Users.Where(x => studentIds.Contains(x.Id))
            .Select(x => new
            {
                Id = x.Id,
                Email = x.Email!,
                Name = x.Name,
                Surname = x.Surname,
            })
            .ToListAsync();
        
        return participants.Select(x => new UserExchangeModel
        {
            Id = x.Id,
            Email = x.Email,
            Name = x.Name,
            Surname = x.Surname,
            Role = UserRole.Student
        }).ToList();
    }
}