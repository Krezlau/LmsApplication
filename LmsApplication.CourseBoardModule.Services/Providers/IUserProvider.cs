using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseBoardModule.Services.Providers;

public interface IUserProvider
{
    Task<UserExchangeModel?> GetUserByIdAsync(string id);
    
    Task<Dictionary<string, UserExchangeModel>> GetUsersByIdsAsync(List<string> ids);
}