using LmsApplication.Core.Shared.Models;

namespace LmsApplication.CourseModule.Services.Providers;

public interface IUserProvider
{
    Task<UserExchangeModel?> GetUserByIdAsync(string id);
}