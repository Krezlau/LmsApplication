using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseBoardModule.Data.Database;
using LmsApplication.CourseModule.Data.Database;
using LmsApplication.CourseModule.Data.Entities;
using LmsApplication.ResourceModule.Data.Database;
using LmsApplication.UserModule.Data.Database;
using LmsApplication.UserModule.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LmsApplication.DataSeeder.Services;

public interface IDataSeederService
{
    Task ClearDatabaseAsync();
    
    Task SeedDataAsync();
}

public class DataSeederService : IDataSeederService
{
    private readonly UserManager<User> _userManager;
    private readonly UserDbContext _userDbContext;
    private readonly IUserStore<User> _userStore;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly CourseDbContext _courseDbContext;
    private readonly CourseBoardDbContext _courseBoardDbContext;
    private readonly ResourceDbContext _resourceDbContext;
    private readonly UserModuleSeederService _userModuleSeederService = new();
    private readonly CourseModuleSeederService _courseModuleSeederService = new();
    private readonly CourseBoardModuleSeederService _courseBoardSeederService = new();
    private readonly Random _random = new();

    public DataSeederService(
        UserManager<User> userManager,
        CourseDbContext courseDbContext,
        CourseBoardDbContext courseBoardDbContext,
        ResourceDbContext resourceDbContext,
        RoleManager<IdentityRole> roleManager,
        UserDbContext userDbContext,
        IUserStore<User> userStore)
    {
        _userManager = userManager;
        _courseDbContext = courseDbContext;
        _courseBoardDbContext = courseBoardDbContext;
        _resourceDbContext = resourceDbContext;
        _roleManager = roleManager;
        _userDbContext = userDbContext;
        _emailStore = (IUserEmailStore<User>)userStore;
        _userStore = userStore;
    }

    public async Task ClearDatabaseAsync()
    {
        Console.WriteLine("Clearing database...");

        var userDbTask = async () =>
        {
            _userDbContext.RoleClaims.RemoveRange(await _userDbContext.RoleClaims.ToListAsync());
            _userDbContext.UserRoles.RemoveRange(await _userDbContext.UserRoles.ToListAsync());
            _userDbContext.UserTokens.RemoveRange(await _userDbContext.UserTokens.ToListAsync());
            _userDbContext.UserLogins.RemoveRange(await _userDbContext.UserLogins.ToListAsync());
            _userDbContext.UserClaims.RemoveRange(await _userDbContext.UserClaims.ToListAsync());
            _userDbContext.Users.RemoveRange(await _userDbContext.Users.ToListAsync());
            _userDbContext.Roles.RemoveRange(await _userDbContext.Roles.ToListAsync());
            await _userDbContext.SaveChangesAsync();

            Console.WriteLine("User database cleared.");
        };

        var courseDbTask = async () =>
        {
            _courseDbContext.CourseCategories.RemoveRange(await _courseDbContext.CourseCategories.ToListAsync());
            _courseDbContext.Courses.RemoveRange(await _courseDbContext.Courses.ToListAsync());
            _courseDbContext.CourseEditions.RemoveRange(await _courseDbContext.CourseEditions.ToListAsync());
            _courseDbContext.CourseEditionParticipants.RemoveRange(await _courseDbContext.CourseEditionParticipants.ToListAsync());
            _courseDbContext.CourseEditionSettings.RemoveRange(await _courseDbContext.CourseEditionSettings.ToListAsync());
            await _courseDbContext.SaveChangesAsync();
            Console.WriteLine("Course database cleared.");
        };

        var courseBoardDbTask = async () =>
        {
            _courseBoardDbContext.PostReactions.RemoveRange(await _courseBoardDbContext.PostReactions.ToListAsync());
            _courseBoardDbContext.CommentReactions.RemoveRange(await _courseBoardDbContext.CommentReactions.ToListAsync());
            _courseBoardDbContext.Comments.RemoveRange(await _courseBoardDbContext.Comments.ToListAsync());
            _courseBoardDbContext.Posts.RemoveRange(await _courseBoardDbContext.Posts.ToListAsync());
            _courseBoardDbContext.GradesTableRowValues.RemoveRange(await _courseBoardDbContext.GradesTableRowValues.ToListAsync());
            _courseBoardDbContext.GradesTableRowTextValues.RemoveRange(await _courseBoardDbContext.GradesTableRowTextValues.ToListAsync());
            _courseBoardDbContext.GradesTableRowNumberValues.RemoveRange(await _courseBoardDbContext.GradesTableRowNumberValues.ToListAsync());
            _courseBoardDbContext.GradesTableRowBoolValues.RemoveRange(await _courseBoardDbContext.GradesTableRowBoolValues.ToListAsync());
            _courseBoardDbContext.GradesTableRowDefinitions.RemoveRange(await _courseBoardDbContext.GradesTableRowDefinitions.ToListAsync());
            _courseBoardDbContext.CourseFinalGrades.RemoveRange(await _courseBoardDbContext.CourseFinalGrades.ToListAsync());
            await _courseBoardDbContext.SaveChangesAsync();
            Console.WriteLine("Course board database cleared.");
        };

        var courseResourceDbTask = async () =>
        {
            _resourceDbContext.ResourceMetadata.RemoveRange(await _resourceDbContext.ResourceMetadata.ToListAsync());
            await _resourceDbContext.SaveChangesAsync();
            Console.WriteLine("Resource database cleared.");
        };
        
        await Task.WhenAll(userDbTask(), courseDbTask(), courseBoardDbTask(), courseResourceDbTask());
    }

    public async Task SeedDataAsync()
    {
        Console.WriteLine("Seeding database...");
        await SeedRolesAsync();
        var (students, teachers, admins) = await SeedUsersAsync();
        Console.WriteLine("Users seeded.");
        
        var courseEditions = await SeedCoursesAsync(students, teachers);
        Console.WriteLine("Courses seeded.");
        
        await SeedPostsAsync(courseEditions);
        Console.WriteLine("Posts seeded.");
        
        await SeedGradesAsync(courseEditions);
        Console.WriteLine("Grades seeded.");
    }

    #region UserModule
    
    private async Task SeedRolesAsync()
    {
        var adminRole = new IdentityRole("Admin");
        var teacherRole = new IdentityRole("Teacher");
        await _roleManager.CreateAsync(adminRole);
        await _roleManager.CreateAsync(teacherRole);
    }

    private async Task<(List<User> students, List<User> teachers, List<User> admins)> SeedUsersAsync()
    {
        var users = _userModuleSeederService.GenerateUsers(120);
        List<User> students = [];
        List<User> teachers = [];
        for (var i = 0; i < users.Count; i++)
        {
            var user = users[i];
            await CreateUserAsync(user, "Qwerty123!@#");
            if (i % 20 == 0)
            {
                await _userManager.AddToRoleAsync(user, "Teacher");
                teachers.Add(user);
                continue;
            }
            students.Add(user);
        }

        var admin = new User
        {
            Name = "Admin",
            Surname = "Admin",
            UserName = "Admin Admin",
            Email = "admin@gmail.com",
        };
        await CreateUserAsync(admin, "Qwerty123!@#");
        await _userManager.AddToRoleAsync(admin, "Admin");
        await _userManager.AddToRoleAsync(admin, "Teacher");
        
        return (students, teachers, [admin]);
    }

    private async Task CreateUserAsync(User user, string password)
    {
         var email = user.Email;
         await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
         await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
         
         var result = await _userManager.CreateAsync(user, password);
         if (!result.Succeeded)
         {
             Console.WriteLine("Error creating user: " + result.Errors.Select(x => x.Description).First());
         }
    }
    
    #endregion
    
    #region CourseModule

    private async Task<List<CourseEdition>> SeedCoursesAsync(List<User> students, List<User> teachers)
    {
        var categories = _courseModuleSeederService.GenerateCourseCategories(20);
        await _courseDbContext.CourseCategories.AddRangeAsync(categories.ToList());
        
        var courses = _courseModuleSeederService.GenerateCourses(30, categories);
        await _courseDbContext.Courses.AddRangeAsync(courses);
        
        var courseEditions = _courseModuleSeederService.GenerateCourseEditions(200, courses);
        await _courseDbContext.CourseEditions.AddRangeAsync(courseEditions);

        foreach (var edition in courseEditions)
        {
            var studentsCount = _random.Next(0, Math.Min(edition.StudentLimit, students.Count));
            var teachersCount = _random.Next(1, 3);
            var studentsToAdd = students.OrderBy(x => _random.Next()).Take(studentsCount).ToList();
            var teachersToAdd = teachers.OrderBy(x => _random.Next()).Take(teachersCount).ToList();
            foreach (var student in studentsToAdd)
            {
                edition.Participants.Add(new CourseEditionParticipant
                {
                    ParticipantId = student.Id,
                    ParticipantRole = UserRole.Student,
                });
            }
            foreach (var teacher in teachersToAdd)
            {
                edition.Participants.Add(new CourseEditionParticipant
                {
                    ParticipantId = teacher.Id,
                    ParticipantRole = UserRole.Teacher,
                });
            }
        }
        
        await _courseDbContext.SaveChangesAsync();
        
        return courseEditions;
    }
    
    #endregion

    #region CourseBoardModule

    private async Task SeedPostsAsync(List<CourseEdition> courseEditions)
    {
        foreach (var edition in courseEditions.Where(x => x.Status is CourseEditionStatus.InProgress or CourseEditionStatus.Finished))
        {
            var posts = _courseBoardSeederService.GeneratePostsWithCommentsAndReactions(_random.Next(1, 100), edition);
            await _courseBoardDbContext.Posts.AddRangeAsync(posts);
        }
        
        await _courseBoardDbContext.SaveChangesAsync();
    }

    private async Task SeedGradesAsync(List<CourseEdition> courseEditions)
    {
        foreach (var edition in courseEditions.Where(x => x.Status is CourseEditionStatus.InProgress or CourseEditionStatus.Finished))
        {
            var gradesTable = _courseBoardSeederService.GenerateGradesTable(_random.Next(1, 10), edition);
            await _courseBoardDbContext.GradesTableRowDefinitions.AddRangeAsync(gradesTable);
        }
        
        await _courseBoardDbContext.SaveChangesAsync();
        
        foreach (var edition in courseEditions.Where(x => x.Status is CourseEditionStatus.InProgress or CourseEditionStatus.Finished))
        {
            var grades = _courseBoardSeederService.GenerateFinalGrades(edition);
            await _courseBoardDbContext.CourseFinalGrades.AddRangeAsync(grades);
        }
        
        await _courseBoardDbContext.SaveChangesAsync();
    }

    #endregion
}