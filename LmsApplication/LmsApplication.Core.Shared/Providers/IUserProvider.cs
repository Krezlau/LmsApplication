using LmsApplication.Core.Shared.Models;

namespace LmsApplication.Core.Shared.Providers;

public interface IUserProvider
{
    Task<UserExchangeModel?> GetUserByEmailAsync(string email);
    
    Task<UserExchangeModel?> GetUserByIdAsync(string id);
}