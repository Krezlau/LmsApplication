using Bogus;
using LmsApplication.Core.Shared.Enums;
using LmsApplication.CourseBoardModule.Data.Entities;
using LmsApplication.CourseModule.Data.Entities;

namespace LmsApplication.DataSeeder.Services;

public class CourseBoardModuleSeederService
{
    private readonly Faker<Post> _postFaker;
    private readonly Faker<Comment> _commentFaker;
    private readonly Faker<GradesTableRowDefinition> _rowDefFaker;
    private readonly Random _random = new();
    private int assignmentCount = 1;

    public CourseBoardModuleSeederService()
    {
        _postFaker = new Faker<Post>()
            .RuleFor(x => x.Content, f => f.Lorem.Sentences(2));
        
        _commentFaker = new Faker<Comment>()
            .RuleFor(x => x.Content, f => f.Lorem.Sentences(2));

        _rowDefFaker = new Faker<GradesTableRowDefinition>()
            .RuleFor(x => x.Title, _ => $"Assignment {assignmentCount++ % 10 + 1}")
            .RuleFor(x => x.Description, f => f.Random.Words(3))
            .RuleFor(x => x.RowType, f => f.PickRandom(Enum.GetValues<RowType>()));
    }
    
    public List<Post> GeneratePostsWithCommentsAndReactions(int count, CourseEdition edition)
    {
        var users = edition.Participants.Select(x => x.ParticipantId).ToList();
        
        _postFaker.RuleFor(x => x.EditionId, _ => edition.Id)
            .RuleFor(x => x.UserId, f => f.PickRandom(users));
        
        var posts = _postFaker.Generate(count);
        
        foreach (var post in posts)
        {
            post.Reactions = GenerateReactions(_random.Next(1, 50), post, users);
            
            _commentFaker.RuleFor(x => x.PostId, _ => post.Id)
                .RuleFor(x => x.UserId, f => f.PickRandom(users));
            var comments = _commentFaker.Generate(_random.Next(1, 5));
            post.Comments = comments;
            
            foreach (var comment in comments)
            {
                comment.Reactions = GenerateReactions(_random.Next(1, 50), comment, users);
            }
        }
        
        return posts;
    }
    
    public List<GradesTableRowDefinition> GenerateGradesTable(int count, CourseEdition edition)
    {
        var users = edition.Participants.Where(x => x.ParticipantRole is UserRole.Student)
            .Select(x => x.ParticipantId)
            .ToList();
        
        var teachers = edition.Participants.Where(x => x.ParticipantRole is UserRole.Teacher)
            .Select(x => x.ParticipantId)
            .ToList();
        
        _rowDefFaker.RuleFor(x => x.CourseEditionId, _ => edition.Id);
        
        var rows = _rowDefFaker.Generate(count);
        
        foreach (var row in rows)
        {
            row.Values = GenerateGrades(row, users, teachers);
        }

        return rows;
    }
    
    public List<FinalGrade> GenerateFinalGrades(CourseEdition edition)
    {
        decimal[] vals = [2.0m, 3.0m, 3.5m, 4.0m, 4.5m, 5.0m];
        var faker = new Faker();
        var grades = new List<FinalGrade>();
        
        var users = edition.Participants.Where(x => x.ParticipantRole is UserRole.Student)
            .Select(x => x.ParticipantId)
            .ToList();
        
        var teachers = edition.Participants.Where(x => x.ParticipantRole is UserRole.Teacher)
            .Select(x => x.ParticipantId)
            .ToList();

        if (edition.Status is CourseEditionStatus.Finished)
        {
            foreach (var user in users)
            {
                var finalGrade = new FinalGrade
                {
                    CourseEditionId = edition.Id,
                    UserId = user,
                    Value =  faker.PickRandom(vals),
                    TeacherId = faker.PickRandom(teachers)
                };
                
                grades.Add(finalGrade);
            }
        }
        else
        {
            foreach (var user in users)
            {
                if (_random.Next(1, 10) < 6)
                {
                    continue;
                }
                
                var finalGrade = new FinalGrade
                {
                    CourseEditionId = edition.Id,
                    UserId = user,
                    Value =  faker.PickRandom(vals),
                    TeacherId = faker.PickRandom(teachers)
                };
                
                grades.Add(finalGrade);
            }
        }
        
        
        return grades;
    }

    private List<GradesTableRowValue> GenerateGrades(GradesTableRowDefinition row, List<string> users, List<string> teachers)
    {
        var faker = new Faker();
        var values = new List<GradesTableRowValue>();
        
        foreach (var user in users)
        {
            if (_random.Next(1, 10) < 3)
            {
                continue;
            }
            switch (row.RowType)
            {
                case RowType.Number:
                    values.Add(new GradesTableRowNumberValue()
                    {
                        UserId = user,
                        Value = faker.Random.Int(1, 10),
                        TeacherId = faker.PickRandom(teachers),
                        TeacherComment = faker.Random.Words(3)
                    });
                    break;
                case RowType.Text:
                    values.Add(new GradesTableRowTextValue()
                    {
                        UserId = user,
                        Value = faker.Random.Word()[..10],
                        TeacherId = faker.PickRandom(teachers),
                        TeacherComment = faker.Random.Words(3)
                    });
                    break;
                case RowType.Bool:
                    values.Add(new GradesTableRowBoolValue()
                    {
                        UserId = user,
                        Value = faker.Random.Bool(),
                        TeacherId = faker.PickRandom(teachers),
                        TeacherComment = faker.Random.Words(3)
                    });
                    break;
            }
            
        }
        
        return values;
    }

    private List<PostReaction> GenerateReactions(int count, Post post, List<string> users)
    {
        var faker = new Faker();
        var reactions = new List<PostReaction>();
        
        for (var i = 0; i < count; i++)
        {
            var reaction = new PostReaction
            {
                PostId = post.Id,
                UserId = faker.PickRandom(users),
                ReactionType = faker.PickRandom(Enum.GetValues<ReactionType>())
            };
            
            reactions.Add(reaction);
        }
        
        return reactions.DistinctBy(x => x.UserId).ToList();
    }
    
    
    private List<CommentReaction> GenerateReactions(int count, Comment comment, List<string> users)
    {
        var faker = new Faker();
        var reactions = new List<CommentReaction>();
        
        for (var i = 0; i < count; i++)
        {
            var reaction = new CommentReaction
            {
                CommentId = comment.Id,
                UserId = faker.PickRandom(users),
                ReactionType = faker.PickRandom(Enum.GetValues<ReactionType>())
            };
            
            reactions.Add(reaction);
        }
        
        return reactions.DistinctBy(x => x.UserId).ToList();
    }
}