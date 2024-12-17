using FluentValidation;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.Core.Shared.Models;
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
    
    Task<CollectionResource<UserModel>> GetUsersAsync(int page, int pageSize);
    
    Task<CollectionResource<UserModel>> GetUsersByCourseEditionAsync(Guid courseEditionId, int page, int pageSize);

    Task<CollectionResource<UserModel>> SearchUsersByEmailAsync(string query, int page, int pageSize);
    
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

    public async Task<CollectionResource<UserModel>> GetUsersAsync(int page, int pageSize)
    {
        var (totalCount, users) = await _userRepository.GetAllUsersAsync(page, pageSize);
        
        return new CollectionResource<UserModel>(users.Select(x => x.ToModel()), totalCount);
    }

    public async Task<CollectionResource<UserModel>> GetUsersByCourseEditionAsync(Guid courseEditionId, int page, int pageSize)
    {
        var userId = _userContext.GetUserId();
        var isAdmin = await _userRepository.IsUserAdminAsync(userId);
        var isParticipant = await _courseEditionProvider.IsUserRegisteredToCourseEditionAsync(courseEditionId, userId);
        
        if (!isAdmin && !isParticipant)
        {
            throw new ValidationException("User is not registered to this course edition.");
        }
        
        var userIds = await _courseEditionProvider.GetCourseEditionParticipantsAsync(courseEditionId);
        
        var (totalCount, users) = await _userRepository.GetUsersByIdsAsync(userIds, page, pageSize);
        
        return new CollectionResource<UserModel>(users.Select(x => x.ToModel()).OrderByDescending(x => x.Role), totalCount);
    }

    public async Task<CollectionResource<UserModel>> SearchUsersByEmailAsync(string query, int page, int pageSize)
    {
        var (totalCount, users) = await _userRepository.SearchUsersByEmailAsync(query, page, pageSize);
        
        return new CollectionResource<UserModel>(users.Select(x => x.ToModel()).OrderByDescending(x => x.Role), totalCount);
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
        
        var isParticipantOfCourses = await _courseEditionProvider.IsUserParticipantOfAnyCourseEditionAsync(userId);
        if (isParticipantOfCourses) 
            throw new ArgumentException("User is a participant of a course edition and cannot be changed.");

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