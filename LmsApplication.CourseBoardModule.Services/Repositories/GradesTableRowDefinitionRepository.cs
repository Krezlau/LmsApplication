using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseBoardModule.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.CourseBoardModule.Services.Repositories;

public interface IGradesTableRowDefinitionRepository
{
    Task<List<GradesTableRowDefinition>> GetGradesTableRowDefinitionsAsync(Guid editionId);
    
    Task<GradesTableRowDefinition?> GetByIdAsync(Guid id);
    
    Task CreateAsync(GradesTableRowDefinition entity);
    
    Task UpdateAsync(GradesTableRowDefinition entity);
    
    Task DeleteAsync(GradesTableRowDefinition entity);
}

public class GradesTableRowDefinitionRepository : IGradesTableRowDefinitionRepository
{
    private readonly CourseBoardDbContext _dbContext;

    public GradesTableRowDefinitionRepository(CourseBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GradesTableRowDefinition>> GetGradesTableRowDefinitionsAsync(Guid editionId)
    {
        return await _dbContext.GradesTableRowDefinitions
            .Where(x => x.CourseEditionId == editionId)
            .ToListAsync();
    }

    public async Task<GradesTableRowDefinition?> GetByIdAsync(Guid id)
    {
        return await _dbContext.GradesTableRowDefinitions.FindAsync(id);
    }

    public async Task CreateAsync(GradesTableRowDefinition entity)
    {
        await _dbContext.GradesTableRowDefinitions.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(GradesTableRowDefinition entity)
    {
        _dbContext.GradesTableRowDefinitions.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(GradesTableRowDefinition entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        _dbContext.GradesTableRowDefinitions.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}