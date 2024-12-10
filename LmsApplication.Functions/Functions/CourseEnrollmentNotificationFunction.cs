using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class CourseEnrollmentNotificationFunction
{
    private readonly ILogger<CourseEnrollmentNotificationFunction> _logger;
    private readonly IEmailService _emailService;

    public CourseEnrollmentNotificationFunction(ILogger<CourseEnrollmentNotificationFunction> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function(nameof(CourseEnrollmentNotificationFunction))]
    public void Run([QueueTrigger(CourseEnrollmentNotificationQueueMessage.QueueName, Connection = "StorageConnection")] CourseEnrollmentNotificationQueueMessage message)
    {
        _logger.LogInformation($"Queue trigger function processed: {message.User.Id}");
        
        _emailService.CreateEmailAsync(message);
    }
}