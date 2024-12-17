using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.UserModule.Services.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id);
    
    Task<User?> GetUserByEmailAsync(string userEmail);
    
    Task<bool> IsUserAdminAsync(string userId);
    
    Task<Dictionary<string, User>> GetUserDictionaryByIdsAsync(List<string> ids);
    
    Task<List<User>> GetUsersByIdsAsync(List<string> studentIds);
    
    Task<(int totalCount, List<User> data)> GetUsersByIdsAsync(List<string> studentIds, int page, int pageSize);
    
    Task<(int totalCount, List<User> data)> GetAllUsersAsync(int page, int pageSize);

    Task<(int totalCount, List<User> data)> SearchUsersByEmailAsync(string query, int page, int pageSize);
    
    Task UpdateAsync(User user);
    
    Task<IList<string>> GetRolesAsync(User user);
    
    Task RemoveFromRolesAsync(User user, IList<string> roles);
    
    Task AddToRoleAsync(User user, string roleName);
    
    Task<IdentityRole?> GetRoleByNameAsync(string? roleName);
}

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly UserDbContext _userDbContext;

    public UserRepository(UserManager<User> userManager, UserDbContext userDbContext)
    {
        _userManager = userManager;
        _userDbContext = userDbContext;
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _userManager.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> IsUserAdminAsync(string userId)
    {
        return await _userManager.IsInRoleAsync(new User {Id = userId }, "Admin");
    }

    public async Task<Dictionary<string, User>> GetUserDictionaryByIdsAsync(List<string> ids)
    {
        return await _userManager.Users
            .Include(x => x.Roles)
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x);
    }

    public async Task<List<User>> GetUsersByIdsAsync(List<string> studentIds)
    {
        return await _userManager.Users.Include(x => x.Roles).Where(x => studentIds.Contains(x.Id))
            .ToListAsync();
    }
    
    public async Task<(int totalCount, List<User> data)> GetUsersByIdsAsync(List<string> studentIds, int page, int pageSize)
    {
        var query = _userManager.Users
            .Include(x => x.Roles)
            .Where(x => studentIds.Contains(x.Id));
        
        var totalCount = await query.CountAsync();
        var data = await query
            .OrderBy(x => x.Roles.Count)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
    }

    public async Task<(int totalCount, List<User> data)> GetAllUsersAsync(int page, int pageSize)
    {
        var query = _userManager.Users
            .Include(x => x.Roles);
        
        var totalCount = await query.CountAsync();
        var data = await query
            .OrderBy(x => x.Roles.Count)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
    }

    public async Task<(int totalCount, List<User> data)> SearchUsersByEmailAsync(string query, int page, int pageSize)
    {
        var q = _userManager.Users
            .Include(x => x.Roles)
            .Where(x => x.NormalizedEmail.Contains(query.ToUpper()));
        
        var totalCount = await q.CountAsync();
        var data = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (totalCount, data);
    }

    public async Task UpdateAsync(User user)
    {
        await _userManager.UpdateAsync(user);
    }

    public async Task<IList<string>> GetRolesAsync(User user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task RemoveFromRolesAsync(User user, IList<string> roles)
    {
        await _userManager.RemoveFromRolesAsync(user, roles);
    }

    public async Task AddToRoleAsync(User user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityRole?> GetRoleByNameAsync(string? roleName)
    {
        if (roleName is null) return null;
        return await _userDbContext.Roles.FirstOrDefaultAsync(x => x.Name!.ToUpper() == roleName);
    }

    public async Task<User?> GetUserByEmailAsync(string userEmail)
    {
        return await _userManager.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == userEmail);
    }
}