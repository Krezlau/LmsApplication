using LmsApplication.CourseBoardModule.Data.Database;

namespace LmsApplication.CourseBoardModule.Services.Repositories;

public interface IGradesTableRowValueRepository
{
    
}

public class GradesTableRowValueRepository : IGradesTableRowValueRepository
{
    private readonly CourseBoardDbContext _context;

    public GradesTableRowValueRepository(CourseBoardDbContext context)
    {
        _context = context;
    }
}