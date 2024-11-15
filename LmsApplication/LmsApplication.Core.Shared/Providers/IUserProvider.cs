using LmsApplication.Core.Shared.Models;

namespace LmsApplication.Core.Shared.Providers;

public interface IUserProvider
{
    Task<UserExchangeModel?> GetUserByEmailAsync(string email);
    
    Task<UserExchangeModel?> GetUserByIdAsync(string id);
    
    Task<Dictionary<string, UserExchangeModel>> GetUsersByIdsAsync(List<string> ids);
    
    Task<bool> IsUserAdminAsync(string userId);
}