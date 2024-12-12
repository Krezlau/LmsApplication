using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Services;
using LmsApplication.UserModule.Data.Mapping;
using LmsApplication.UserModule.Data.Models;
using LmsApplication.UserModule.Services.Providers;
using LmsApplication.UserModule.Services.Repositories;
using Microsoft.AspNetCore.Identity;

namespace LmsApplication.UserModule.Services.Services;

public interface IUserService
{
    Task<UserModel> GetCurrentUserAsync();
    
    Task<UserModel> GetUserByEmailAsync(string userEmail);
    
    Task<List<UserModel>> GetUsersAsync();
    
    Task<List<UserModel>> GetUsersByCourseEditionAsync(Guid courseEditionId);

    Task<List<UserModel>> SearchUsersByEmailAsync(string query);
    
    Task UpdateUserAsync(string userId, UserUpdateModel model);

    Task UpdateUserRoleAsync(string userId, UpdateUserRoleModel model);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseEditionProvider _courseEditionProvider;
    private readonly IValidationService<UserUpdateModel> _validationService;
    private readonly IUserContext _userContext;

    public UserService(
        IValidationService<UserUpdateModel> validationService,
        ICourseEditionProvider courseEditionProvider,
        IUserContext userContext,
        IUserRepository userRepository)
    {
        _validationService = validationService;
        _courseEditionProvider = courseEditionProvider;
        _userContext = userContext;
        _userRepository = userRepository;
    }

    public async Task<UserModel> GetCurrentUserAsync()
    {
        var userId = _userContext.GetUserId();
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) 
            throw new KeyNotFoundException("Couldn't find user with the given id.");
        
        return user.ToModel();
    }

    public async Task<UserModel> GetUserByEmailAsync(string userEmail)
    {
        var user = await _userRepository.GetUserByEmailAsync(userEmail);
        if (user is null)
            throw new KeyNotFoundException("Couldn't find user with the given email.");

        return user.ToModel();
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        
        return users.Select(x => x.ToModel()).OrderByDescending(x => x.Role).ToList();
    }

    public async Task<List<UserModel>> GetUsersByCourseEditionAsync(Guid courseEditionId)
    {
        var userId = _userContext.GetUserId();
        var isAdmin = await _userRepository.IsUserAdminAsync(userId);
        var isParticipant = await _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(courseEditionId, userId);
        
        if (!isAdmin && !isParticipant)
        {
            throw new ValidationException("User is not registered to this course edition.");
        }
        
        var userIds = await _courseEditionProvider.GetCourseEditionParticipantsAsync(courseEditionId);
        
        var users = await _userRepository.GetUsersByIdsAsync(userIds);
        
        return users.Select(x => x.ToModel()).OrderByDescending(x => x.Role).ToList();
    }

    public async Task<List<UserModel>> SearchUsersByEmailAsync(string query)
    {
        var users = await _userRepository.SearchUsersByEmailAsync(query);
        
        return users.Select(x => x.ToModel()).OrderByDescending(x => x.Role).ToList();
    }

    public async Task UpdateUserAsync(string userId, UserUpdateModel model)
    {
        await _validationService.ValidateAndThrowAsync(model);
        
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) 
            throw new KeyNotFoundException("Couldn't find user with the given id.");
        
        user.Name = model.Name;
        user.Surname = model.Surname;
        user.Bio = model.Bio;
        
        await _userRepository.UpdateAsync(user);
    }

    public async Task UpdateUserRoleAsync(string userId, UpdateUserRoleModel model)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null) 
            throw new KeyNotFoundException("Couldn't find user with the given id.");

        if (model.Role is UserRole.Student)
        {
            await _userRepository.RemoveFromRolesAsync(user, await _userRepository.GetRolesAsync(user));
            return;
        }
        var role = await GetRoleByEnumAsync(model.Role);
        
        await _userRepository.AddToRoleAsync(user, role.Name!);
    }
    
    private async Task<IdentityRole> GetRoleByEnumAsync(UserRole role)
    {
        var roleName = Enum.GetName(role)?.ToUpper();
        var identityRole = await _userRepository.GetRoleByNameAsync(roleName);
        if (identityRole is null)
        {
            throw new KeyNotFoundException("Couldn't find the role.");
        }

        return identityRole;
    }
}