using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseBoardModule.Services.Repositories;

public interface IGradesTableRowValueRepository
{
    Task<Dictionary<Guid, GradesTableRowValue>> GetGradesTableRowValuesAsync(string userId, IEnumerable<Guid> rowIds);
    
    Task<GradesTableRowValue?> GetGradesTableRowValueAsync(Guid rowId, string userId);
    
    Task AddAsync(GradesTableRowValue grade);
    
    Task UpdateAsync(GradesTableRowValue grade);
    
    Task DeleteAsync(GradesTableRowValue grade);
}

public class GradesTableRowValueRepository : IGradesTableRowValueRepository
{
    private readonly CourseBoardDbContext _context;

    public GradesTableRowValueRepository(CourseBoardDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<Guid, GradesTableRowValue>> GetGradesTableRowValuesAsync(string userId, IEnumerable<Guid> rowIds)
    {
        return await _context.GradesTableRowValues
            .Where(x => x.UserId == userId && rowIds.Contains(x.RowDefinitionId))
            .ToDictionaryAsync(x => x.RowDefinitionId, x => x);
    }

    public async Task<GradesTableRowValue?> GetGradesTableRowValueAsync(Guid rowId, string userId)
    {
        return await _context.GradesTableRowValues
            .FirstOrDefaultAsync(x => x.RowDefinitionId == rowId && x.UserId == userId);
    }

    public async Task AddAsync(GradesTableRowValue grade)
    {
        await _context.GradesTableRowValues.AddAsync(grade);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(GradesTableRowValue grade)
    {
        _context.GradesTableRowValues.Update(grade);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(GradesTableRowValue grade)
    {
        grade.IsDeleted = true;
        grade.DeletedAtUtc = DateTime.UtcNow;
        _context.GradesTableRowValues.Update(grade);
        await _context.SaveChangesAsync();
    }
}