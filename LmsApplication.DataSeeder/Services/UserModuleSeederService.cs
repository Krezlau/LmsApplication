using Bogus;
using LmsApplication.UserModule.Data.Entities;

namespace LmsApplication.DataSeeder.Services;

public class UserModuleSeederService
{
    private readonly Faker<User> _userFaker;

    private static readonly string[] Roles = {"Admin", "Teacher"};
    
    public UserModuleSeederService()
    {
        _userFaker = new Faker<User>()
            .StrictMode(false)
            .RuleFor(x => x.Name, f => f.Person.FirstName)
            .RuleFor(x => x.Surname, f => f.Person.LastName)
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Bio, f => f.Lorem.Paragraphs(1, 3))
            .RuleFor(x => x.UserName, f => f.Person.UserName);
    }

    public List<User> GenerateUsers(int count)
    {
        return _userFaker.Generate(count);
    }
}