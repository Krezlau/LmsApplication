using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Services;

public interface IEmailService
{
    Task SendEmailAsync(string recipient, string title, string body);
}

public class EmailService : IEmailService
{
    private const string FromAddress = "lmsapplication@contoso.com";
    
    private readonly SmtpClient _smtpClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(SmtpClient smtpClient, ILogger<EmailService> logger)
    {
        _smtpClient = smtpClient;
        _logger = logger;
    }

    public async Task SendEmailAsync(string recipient, string title, string body)
    {
        var message = new MailMessage(FromAddress, recipient, title, body);
        message.IsBodyHtml = true;
        
        await _smtpClient.SendMailAsync(message);
        
        _logger.LogInformation($"Email sent to {recipient} with title {title}");
    }
}