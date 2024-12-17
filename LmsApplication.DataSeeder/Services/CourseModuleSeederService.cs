using Bogus;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseModule.Data.Entities;

namespace LmsApplication.DataSeeder.Services;

public class CourseModuleSeederService
{
    private readonly Faker<Course> _courseFaker;
    private readonly Faker<CourseEdition> _courseEditionFaker;
    private readonly Faker<CourseCategory> _courseCategoryFaker;
    private readonly Random _random = new();
    
    private static List<string> courseCategories = new () 
    {
         "Computer Science",
         "Mathematics",
         "Biology",
         "Physics",
         "Chemistry",
         "Economics",
         "Psychology",
         "Sociology",
         "History",
         "Political Science",
         "Engineering",
         "Philosophy",
         "Literature",
         "Business Administration",
         "Fine Arts"
     };
    
    private static List<string> courseTitles = new() 
    {
        "Introduction to Computer Programming",
        "Data Structures and Algorithms",
        "Machine Learning Fundamentals",
        "Calculus I",
        "Linear Algebra",
        "Organic Chemistry",
        "Physics for Engineers",
        "Environmental Science",
        "Principles of Microeconomics",
        "Introduction to Psychology",
        "Sociological Theory",
        "World History: 1500 to Present",
        "American Government",
        "Ethics and Moral Philosophy",
        "Shakespearean Literature",
        "Business Management Principles",
        "Marketing Strategies",
        "Digital Media and Design",
        "Introduction to Artificial Intelligence",
        "Mobile App Development",
        "Database Management Systems",
        "Biochemistry",
        "Genetics and Evolution",
        "Thermodynamics",
        "Statistics and Probability",
        "Creative Writing Workshop",
        "Public Speaking and Communication",
        "International Relations",
        "Cognitive Psychology",
        "Project Management Fundamentals"
    };

    public CourseModuleSeederService()
    {
        _courseFaker = new Faker<Course>()
            .RuleFor(x => x.Title, f => f.PickRandom(courseTitles))
            .RuleFor(x => x.Description, f => f.Lorem.Paragraph())
            .RuleFor(x => x.Duration, f => CourseDuration.TwoSemesters);

        _courseEditionFaker = new Faker<CourseEdition>()
            .RuleFor(x => x.Title, f => $"{f.Random.Word()} Group")
            .RuleFor(x => x.StudentLimit, f => f.Random.Number(10, 120));

        _courseCategoryFaker = new Faker<CourseCategory>()
            .RuleFor(x => x.Name, f => f.PickRandom(courseCategories));
    }
    
    public List<Course> GenerateCourses(int count, List<CourseCategory> courseCategories)
    {
        _courseFaker.RuleFor(x => x.Categories, f => f.PickRandom(courseCategories, f.Random.Number(1, 3)).ToList());
        return _courseFaker.Generate(count).DistinctBy(x => x.Title).ToList();
    }
    
    public List<CourseEdition> GenerateCourseEditions(int count, List<Course> courses)
    {
        _courseEditionFaker.RuleFor(x => x.CourseId, f => f.PickRandom(courses).Id);
        var finished = _courseEditionFaker.Generate(count / 5);
        foreach (var courseEdition in finished)
        {
            var startDate = DateTime.UtcNow.AddYears(-1).AddDays(-_random.Next(1, 365));
            courseEdition.StartDateUtc = startDate;
        }
        
        var inProgress = _courseEditionFaker.Generate(count / 5);
        foreach (var courseEdition in inProgress)
        {
            var startDate = DateTime.UtcNow.AddDays(-_random.Next(1, 365));
            courseEdition.StartDateUtc = startDate;
        }
        
        var planned = _courseEditionFaker.Generate(count / 5);
        foreach (var courseEdition in planned)
        {
            var startDate = DateTime.UtcNow.AddDays(_random.Next(1, 365));
            courseEdition.StartDateUtc = startDate;
        }
        
        var registrationOpen = _courseEditionFaker.Generate(count / 5);
        foreach (var courseEdition in registrationOpen)
        {
            var startDate = DateTime.UtcNow.AddDays(_random.Next(60, 365));
            courseEdition.StartDateUtc = startDate;
            courseEdition.RegistrationStartDateUtc = DateTime.UtcNow.AddDays(_random.Next(-14, 29));
            courseEdition.RegistrationEndDateUtc = courseEdition.RegistrationStartDateUtc.Value.AddDays(_random.Next(1, 30));
        }
        
        var registrationClosed = _courseEditionFaker.Generate(count / 5 + count % 5);
        foreach (var courseEdition in registrationClosed)
        {
            var startDate = DateTime.UtcNow.AddDays(_random.Next(60, 365));
            courseEdition.StartDateUtc = startDate;
            courseEdition.RegistrationStartDateUtc = DateTime.UtcNow.AddDays(-30);
            courseEdition.RegistrationEndDateUtc = DateTime.UtcNow.AddDays(-1);
        }
        
        return finished.Concat(inProgress).Concat(planned).Concat(registrationOpen).Concat(registrationClosed).ToList();
    }
    
    public List<CourseCategory> GenerateCourseCategories(int count)
    {
        return _courseCategoryFaker.Generate(count).DistinctBy(x => x.Name).ToList();
    }
}