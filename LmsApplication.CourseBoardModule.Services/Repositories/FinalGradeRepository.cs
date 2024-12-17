using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseBoardModule.Services.Repositories;

public interface IFinalGradeRepository
{
    Task<FinalGrade?> GetFinalGradeAsync(Guid editionId, string studentId);
    
    Task<bool> GradeExistsAsync(Guid courseEditionId, string studentId);
    
    Task<List<FinalGrade>> GetUserFinalGradesAsync(string userId);
    
    Task CreateAsync(FinalGrade finalGrade);
    
    Task UpdateAsync(FinalGrade finalGrade);
    
    Task DeleteAsync(FinalGrade finalGrade);
}

public class FinalGradeRepository : IFinalGradeRepository
{
    private readonly CourseBoardDbContext _context;

    public FinalGradeRepository(CourseBoardDbContext context)
    {
        _context = context;
    }
    
    public async Task<FinalGrade?> GetFinalGradeAsync(Guid editionId, string studentId)
    {
        return await _context.CourseFinalGrades
            .Where(x => x.CourseEditionId == editionId && x.UserId == studentId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> GradeExistsAsync(Guid courseEditionId, string studentId)
    {
        return await _context.CourseFinalGrades
            .AnyAsync(x => x.CourseEditionId == courseEditionId && x.UserId == studentId);
    }

    public async Task<List<FinalGrade>> GetUserFinalGradesAsync(string userId)
    {
        return await _context.CourseFinalGrades
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task CreateAsync(FinalGrade finalGrade)
    {
        await _context.CourseFinalGrades.AddAsync(finalGrade);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FinalGrade finalGrade)
    {
        _context.CourseFinalGrades.Update(finalGrade);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(FinalGrade finalGrade)
    {
        _context.CourseFinalGrades.Remove(finalGrade);
        await _context.SaveChangesAsync();
    }
}