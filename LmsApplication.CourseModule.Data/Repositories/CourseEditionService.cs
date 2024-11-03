using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Data.Repositories;

public interface ICourseEditionService
{
    Task<List<CourseEdition>> GetAllCourseEditionsAsync();
    
    Task<List<CourseEdition>> GetCourseEditionsByCourseIdAsync(Guid courseId);
    
    Task<List<CourseEdition>> GetUserCourseEditionsAsync(string userEmail);
    
    Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id);
    
    Task AddParticipantToCourseEditionAsync(Guid courseEditionId, string userEmail, UserRole userRole);

    Task CreateAsync(CourseEdition courseEdition);
    
    Task UpdateAsync(CourseEdition courseEdition);
}

public class CourseEditionService : ICourseEditionService
{
    private readonly CourseDbContext _context;

    public CourseEditionService(CourseDbContext context)
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

    public async Task<List<CourseEdition>> GetUserCourseEditionsAsync(string userEmail)
    {
        // todo pagination
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Where(x => x.EndDateUtc < DateTime.UtcNow && x.Participants.Any(p => p.ParticipantEmail == userEmail))
            .ToListAsync();
    }

    public async Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id)
    {
        return await _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddParticipantToCourseEditionAsync(Guid courseEditionId, string userEmail, UserRole userRole)
    {
        var participant = new CourseEditionParticipant()
        {
            CourseEditionId = courseEditionId,
            ParticipantEmail = userEmail,
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