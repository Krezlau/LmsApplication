using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using Microsoft.Extensions.Configuration;

namespace LmsApplication.Functions.Services;

public interface IEmailCreateService
{
    Task CreateEmailAsync(CourseEnrollmentNotificationQueueMessage message);
    
    Task CreateEmailAsync(GradeNotificationQueueMessage message);
    
    Task CreateEmailAsync(PostNotificationQueueMessage message);
}

public class EmailCreateService : IEmailCreateService
{
    private readonly IQueueClient<NotificationQueueMessage> _queueClient;
    private readonly string _appUrl;

    public EmailCreateService(IQueueClient<NotificationQueueMessage> queueClient, IConfiguration config)
    {
        _queueClient = queueClient;
        _appUrl = config["AppUrl"] ?? "www.lmsapp.pl";
    }

    public async Task CreateEmailAsync(CourseEnrollmentNotificationQueueMessage message)
    {
        var notificationMessage = new NotificationQueueMessage
        {
            // Recipient = message.User.Email,
            Recipient = "krzysztof.andrzej.jurkowski@gmail.com",
            Title= $"You have been enrolled in a \"{message.CourseName}\" course!",
            Body = $"You have been enrolled in the course " +
                   $"<a href=\"{_appUrl}/editions/{message.CourseEditionId}\">{message.CourseEditionName}</a>" +
                   $"at {message.TimeStampUtc:f} UTC."
        };

        await _queueClient.EnqueueAsync(notificationMessage);
    }

    public async Task CreateEmailAsync(GradeNotificationQueueMessage message)
    {
        var grade = message.RowType switch
        {
            RowType.Bool => bool.TryParse(message.Grade, out var boolGrade) ? boolGrade ? "Yes" : "No" : message.Grade,
            RowType.Number => message.Grade,
            RowType.Text => message.Grade,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var notificationMessage = new NotificationQueueMessage
        {
            // Recipient = message.User.Email,
            Recipient = "krzysztof.andrzej.jurkowski@gmail.com",
            Title = $"{message.CourseEditionName}: You have a new grade!",
            Body = $"You have received a new grade for the course" +
                   $" <a href=\"{_appUrl}/editions/{message.CourseEditionId}\">{message.CourseEditionName}</a>.<br />" +
                   $"Row: \"{message.RowName}\", Value: \"{grade}\"" +
                   $" by {message.Teacher.Name} {message.Teacher.Surname} at {message.TimeStampUtc:f} UTC. <br />"
        };
        if (!string.IsNullOrWhiteSpace(message.TeacherComment))
        {
            notificationMessage.Body += $"With comment: {message.TeacherComment}";
        }

        await _queueClient.EnqueueAsync(notificationMessage);
    }

    public async Task CreateEmailAsync(PostNotificationQueueMessage message)
    {
        var notificationMessage = new NotificationQueueMessage
        {
            // Recipient = message.User.Email,
            Recipient = "krzysztof.andrzej.jurkowski@gmail.com",
            Title = $"{message.CourseEditionName}: New post in the course!",
            Body = $"There is a new post in the course " +
                   $"<a href=\"{_appUrl}/editions/{message.CourseEditionId}\">{message.CourseEditionName}</a> " +
                   $"by {message.Poster.Name} {message.Poster.Surname} at {message.TimeStampUtc:f} UTC. <br />" +
                   $"\"{message.PostBody}\""
        };

        await _queueClient.EnqueueAsync(notificationMessage);
    }
}