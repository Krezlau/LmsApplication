using System.Net.Mail;
using LmsApplication.Core.Shared.QueueMessages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Services;

public interface IEmailService
{
    Task CreateEmailAsync(CourseEnrollmentNotificationQueueMessage message);
    
    Task CreateEmailAsync(GradeNotificationQueueMessage message);
    
    Task CreateEmailAsync(PostNotificationQueueMessage message);
    
    Task CreateEmailAsync(FinalGradeNotificationQueueMessage message);
}

public class EmailService : IEmailService
{
    private const string FromAddress = "lmsapplication@contoso.com";
    
    private readonly SmtpClient _smtpClient;
    private readonly ILogger<EmailService> _logger;
    private readonly string _appUrl;
    private readonly bool _isTestMode;

    public EmailService(SmtpClient smtpClient, ILogger<EmailService> logger, IConfiguration config)
    {
        _smtpClient = smtpClient;
        _logger = logger;
        _appUrl = config["AppUrl"] ?? "www.lmsapp.pl";
        _isTestMode = config.GetValue<bool>("TestMode");
    }

    public async Task CreateEmailAsync(CourseEnrollmentNotificationQueueMessage message)
    {
        var recipient = _isTestMode ? "krzysztof.andrzej.jurkowski@gmail.com" : message.User.Email;
        var title = $"You have been enrolled in a \"{message.CourseName}\" course!";
        var body = $"You have been enrolled in the course " +
               $"<a href=\"{_appUrl}/editions/{message.CourseEditionId}\">{message.CourseEditionName}</a> " +
               $"at {message.TimeStampUtc:f} UTC.";

        await SendEmailAsync(recipient, title, body);
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
        
        var recipient = _isTestMode ? "krzysztof.andrzej.jurkowski@gmail.com" : message.User.Email;
        var title = $"{message.CourseEditionName}: You have a new grade!";
        var body = $"You have received a new grade for the course" +
                   $" <a href=\"{_appUrl}/editions/{message.CourseEditionId}\">{message.CourseEditionName}</a>.<br />" +
                   $"Row: \"{message.RowName}\", Value: \"{grade}\"" +
                   $" by {message.Teacher.Name} {message.Teacher.Surname} at {message.TimeStampUtc:f} UTC. <br />";
        
        if (!string.IsNullOrWhiteSpace(message.TeacherComment))
        {
            body += $"With comment: {message.TeacherComment}";
        }

        await SendEmailAsync(recipient, title, body);
    }

    public async Task CreateEmailAsync(PostNotificationQueueMessage message)
    {
        var recipient = _isTestMode ? "krzysztof.andrzej.jurkowski@gmail.com" : message.User.Email;
        var title = $"{message.CourseEditionName}: New post in the course!";
        var body = $"There is a new post in the course " +
               $"<a href=\"{_appUrl}/editions/{message.CourseEditionId}\">{message.CourseEditionName}</a> " +
               $"by {message.Poster.Name} {message.Poster.Surname} at {message.TimeStampUtc:f} UTC. <br />" +
               $"\"{message.PostBody}\"";

        await SendEmailAsync(recipient, title, body);
    }

    public async Task CreateEmailAsync(FinalGradeNotificationQueueMessage message)
    {
        var recipient = _isTestMode ? "krzysztof.andrzej.jurkowski@gmail.com" : message.User.Email;
        var title = $"{message.CourseEditionName}: You received a final grade from the course!";
        var body = $"You have received a final grade for the course" +
                   $" <a href=\"{_appUrl}/editions/{message.CourseEditionId}\">{message.CourseEditionName}</a>.<br />" +
                   $"Your final grade: \"{message.Grade}\"" +
                   $" by {message.Teacher.Name} {message.Teacher.Surname} at {message.TimeStampUtc:f} UTC. <br />";
        
        await SendEmailAsync(recipient, title, body);
    }

    private async Task SendEmailAsync(string recipient, string title, string body)
    {
        var message = new MailMessage(FromAddress, recipient, title, body);
        message.IsBodyHtml = true;
        
        await _smtpClient.SendMailAsync(message);
        
        _logger.LogInformation($"Email sent to {recipient} with title {title}");
    }
}