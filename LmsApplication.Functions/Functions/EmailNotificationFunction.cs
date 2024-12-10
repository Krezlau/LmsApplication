using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class EmailNotificationFunction
{
    private readonly ILogger<EmailNotificationFunction> _logger;
    private readonly IEmailService _emailService;

    public EmailNotificationFunction(ILogger<EmailNotificationFunction> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function(nameof(EmailNotificationFunction))]
    public async Task Run([QueueTrigger(NotificationQueueMessage.QueueName, Connection = "StorageConnection")] NotificationQueueMessage message)
    {
        _logger.LogInformation($"Queue trigger function processed: {message.Title}");
        await _emailService.SendEmailAsync(message.Recipient, message.Title, message.Body);
    }
}