using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Services.Repositories;

public interface ICourseEditionRepository
{
    Task<List<CourseEdition>> GetAllCourseEditionsAsync();
    
    Task<List<CourseEdition>> GetCourseEditionsByCourseIdAsync(Guid courseId);
    
    Task<List<CourseEdition>> GetUserCourseEditionsAsync(string userId);

    Task<CourseEdition?> GetCourseEditionByCourseIdAndTitleAsync(string title, Guid courseId);
    
    Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id);
    
    Task AddParticipantToCourseEditionAsync(Guid courseEditionId, string userId, UserRole userRole);

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
            .ToListAsync();
    }

    public async Task<List<CourseEdition>> GetCourseEditionsByCourseIdAsync(Guid courseId)
    {
        // todo pagination
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Where(x => x.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<List<CourseEdition>> GetUserCourseEditionsAsync(string userId)
    {
        // todo pagination
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Where(x => x.EndDateUtc < DateTime.UtcNow && x.Participants.Any(p => p.ParticipantId == userId))
            .ToListAsync();
    }

    public async Task<CourseEdition?> GetCourseEditionByCourseIdAndTitleAsync(string title, Guid courseId)
    {
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .FirstOrDefaultAsync(x => x.CourseId == courseId && x.Title == title);
    }

    public async Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id)
    {
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
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