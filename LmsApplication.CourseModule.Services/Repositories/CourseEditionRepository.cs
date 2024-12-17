using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseModule.Services.Repositories;

public interface ICourseEditionRepository
{
    Task<(int totalCount, List<CourseEdition> data)> GetAllCourseEditionsAsync(int page, int pageSize);
    
    Task<(int totalCount, List<CourseEdition> data)> GetCourseEditionsByCourseIdAsync(Guid courseId, int page, int pageSize);

    Task<(int totalCount, List<CourseEdition> data)> GetEditionsWithRegistrationOpenAsync(string userId, Guid? courseId, int page, int pageSize);
    
    Task<(int totalCount, List<CourseEdition> data)> GetUserCourseEditionsAsync(string userId, Guid? courseId, int page, int pageSize);
    
    Task<(int totalCount, List<CourseEdition> data)> GetCourseEditionsByUserIdAsync(string userId, int page, int pageSize);
    
    Task<(int totalCount, List<CourseEdition> data)> GetMutualCourseEditionsAsync(string userId, string userId2, int page, int pageSize);

    Task<CourseEdition?> GetCourseEditionByCourseIdAndTitleAsync(string title, Guid courseId);
    
    Task<CourseEdition?> GetCourseEditionByIdAsync(Guid id);

    Task<bool> CourseEditionExistsAsync(Guid courseEditionId);
    
    Task<bool> IsUserParticipantInAnyCourseEditionAsync(string userId);
    
    Task<List<string>> GetCourseEditionParticipantIdsAsync(Guid courseEditionId, UserRole? userRole = null);
    
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

    public async Task<(int totalCount, List<CourseEdition> data)> GetAllCourseEditionsAsync(int page, int pageSize)
    {
        var query = _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings);
        
        var totalCount = await query.CountAsync();
        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
    }

    public async Task<(int totalCount, List<CourseEdition> data)> GetCourseEditionsByCourseIdAsync(Guid courseId, int page, int pageSize)
    {
        var query = _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.CourseId == courseId);
        
        var totalCount = await query.CountAsync();
        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
    }

    public async Task<(int totalCount, List<CourseEdition> data)> GetEditionsWithRegistrationOpenAsync(string userId, Guid? courseId, int page, int pageSize)
    {
        var userFinishedCourses = await _context.CourseEditions
            .Include(x => x.Participants)
            .Where(x => x.Participants.Any(p => p.ParticipantId == userId))
            .Select(x => x.CourseId)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var query = _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.RegistrationEndDateUtc != null &&
                        x.RegistrationEndDateUtc > now &&
                        x.RegistrationStartDateUtc != null &&
                        x.RegistrationStartDateUtc < now &&
                        !userFinishedCourses.Contains(x.CourseId) &&
                        x.Participants.Count < x.StudentLimit);
        
        if (courseId is not null) 
            query = query.Where(x => x.CourseId == courseId);
        
        var totalCount = await query.CountAsync();
        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
    }

    public async Task<(int totalCount, List<CourseEdition> data)> GetUserCourseEditionsAsync(string userId, Guid? courseId, int page, int pageSize)
    {
        var query = _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.EndDateUtc > DateTime.UtcNow && x.Participants.Any(p => p.ParticipantId == userId));
        
        if (courseId is not null) 
            query = query.Where(x => x.CourseId == courseId);
        
        var totalCount = await query.CountAsync();
        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
    }

    public async Task<(int totalCount, List<CourseEdition> data)> GetCourseEditionsByUserIdAsync(string userId, int page, int pageSize)
    {
        var query = _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.Participants.Any(p => p.ParticipantId == userId));
        
        var totalCount = await query.CountAsync();
        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
    }

    public async Task<(int totalCount, List<CourseEdition> data)> GetMutualCourseEditionsAsync(string userId, string userId2, int page, int pageSize)
    {
        var query = _context.CourseEditions
            .Include(x => x.Course)
            .Include(x => x.Participants)
            .Include(x => x.Settings)
            .Where(x => x.Participants.Any(p => p.ParticipantId == userId) &&
                        x.Participants.Any(p => p.ParticipantId == userId2));
        
        var totalCount = await query.CountAsync();
        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
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

    public async Task<bool> CourseEditionExistsAsync(Guid courseEditionId)
    {
        return await _context.CourseEditions.AnyAsync(x => x.Id == courseEditionId);
    }

    public async Task<bool> IsUserParticipantInAnyCourseEditionAsync(string userId)
    {
        return await _context.CourseEditionParticipants.AnyAsync(x => x.ParticipantId == userId);
    }

    public async Task<List<string>> GetCourseEditionParticipantIdsAsync(Guid courseEditionId, UserRole? userRole = null)
    {
        var query = _context.CourseEditionParticipants
            .Where(x => x.CourseEditionId == courseEditionId);
        
        if (userRole is not null)
            query = query.Where(x => x.ParticipantRole == userRole);
        
        return await query.Select(x => x.ParticipantId).ToListAsync();
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