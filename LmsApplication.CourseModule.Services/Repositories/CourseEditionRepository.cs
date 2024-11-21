using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Services.Repositories;

public interface ICourseEditionRepository
{
    Task<List<CourseEdition>> GetAllCourseEditionsAsync();
    
    Task<List<CourseEdition>> GetCourseEditionsByCourseIdAsync(Guid courseId);

    Task<List<CourseEdition>> GetEditionsWithRegistrationOpenAsync(string userId);
    
    Task<List<CourseEdition>> GetUserCourseEditionsAsync(string userId);
    
    Task<List<CourseEdition>> GetCourseEditionsByUserIdAsync(string userId);
    
    Task<List<CourseEdition>> GetMutualCourseEditionsAsync(string userId, string userId2);

    Task<CourseEdition?> GetCourseEditionByCourseIdAndTitleAsync(string title, Guid courseId);
    
    Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id);
    
    Task AddParticipantToCourseEditionAsync(Guid courseEditionId, string userId, UserRole userRole);
    
    Task RemoveParticipantFromCourseEditionAsync(Guid courseEditionId, string userId);
    
    Task<bool> IsUserParticipantInCourseEditionAsync(Guid courseEditionId, string userId);

    Task CreateAsync(CourseEdition courseEdition);
    
    Task UpdateAsync(CourseEdition courseEdition);
}

public class CourseEditionRepository : ICourseEditionRepository
{
    private readonly CourseDbContext _context;

    public CourseEditionRepository(CourseDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseEdition>> GetAllCourseEditionsAsync()
    {
        // todo pagination
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .ToListAsync();
    }

    public async Task<List<CourseEdition>> GetCourseEditionsByCourseIdAsync(Guid courseId)
    {
        // todo pagination
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<List<CourseEdition>> GetEditionsWithRegistrationOpenAsync(string userId)
    {
        var userFinishedCourses = await _context.CourseEditions
            .Include(x => x.Participants)
            .Where(x => x.Participants.Any(p => p.ParticipantId == userId))
            .Select(x => x.CourseId)
            .ToListAsync();

        var now = DateTime.UtcNow;
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.RegistrationEndDateUtc != null &&
                        x.RegistrationEndDateUtc > now &&
                        x.RegistrationStartDateUtc != null &&
                        x.RegistrationStartDateUtc < now &&
                        !userFinishedCourses.Contains(x.CourseId) &&
                        x.Participants.Count < x.StudentLimit)
            .ToListAsync();
    }

    public async Task<List<CourseEdition>> GetUserCourseEditionsAsync(string userId)
    {
        // todo pagination
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.EndDateUtc > DateTime.UtcNow && x.Participants.Any(p => p.ParticipantId == userId))
            .ToListAsync();
    }

    public async Task<List<CourseEdition>> GetCourseEditionsByUserIdAsync(string userId)
    {
        // todo pagination
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.Participants.Any(p => p.ParticipantId == userId))
            .ToListAsync();
    }

    public async Task<List<CourseEdition>> GetMutualCourseEditionsAsync(string userId, string userId2)
    {
        // todo pagination
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.Participants.Any(p => p.ParticipantId == userId) &&
                        x.Participants.Any(p => p.ParticipantId == userId2))
            .ToListAsync();
    }

    public async Task<CourseEdition?> GetCourseEditionByCourseIdAndTitleAsync(string title, Guid courseId)
    {
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .FirstOrDefaultAsync(x => x.CourseId == courseId && x.Title == title);
    }

    public async Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id)
    {
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddParticipantToCourseEditionAsync(Guid courseEditionId, string userId, UserRole userRole)
    {
        var participant = new CourseEditionParticipant
        {
            CourseEditionId = courseEditionId,
            ParticipantId = userId,
            ParticipantRole = userRole
        };
        
        await _context.CourseEditionParticipants.AddAsync(participant);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveParticipantFromCourseEditionAsync(Guid courseEditionId, string userId)
    {
        var participant = await _context.CourseEditionParticipants
            .FirstOrDefaultAsync(x => x.CourseEditionId == courseEditionId && x.ParticipantId == userId);

        if (participant is not null)
        {
            _context.CourseEditionParticipants.Remove(participant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsUserParticipantInCourseEditionAsync(Guid courseEditionId, string userId)
    {
        return await _context.CourseEditionParticipants
            .AnyAsync(x => x.CourseEditionId == courseEditionId && x.ParticipantId == userId);
    }

    public async Task CreateAsync(CourseEdition courseEdition)
    {
        await _context.CourseEditions.AddAsync(courseEdition);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CourseEdition courseEdition)
    {
        _context.CourseEditions.Update(courseEdition);
        await _context.SaveChangesAsync();
    }
}