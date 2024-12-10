using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class CourseEnrollmentNotificationFunction
{
    private readonly ILogger<CourseEnrollmentNotificationFunction> _logger;
    private readonly IEmailCreateService _emailCreateService;

    public CourseEnrollmentNotificationFunction(ILogger<CourseEnrollmentNotificationFunction> logger, IEmailCreateService emailCreateService)
    {
        _logger = logger;
        _emailCreateService = emailCreateService;
    }

    [Function(nameof(CourseEnrollmentNotificationFunction))]
    public void Run([QueueTrigger(CourseEnrollmentNotificationQueueMessage.QueueName, Connection = "StorageConnection")] CourseEnrollmentNotificationQueueMessage message)
    {
        _logger.LogInformation($"Queue trigger function processed: {message.User.Id}");
        
        _emailCreateService.CreateEmailAsync(message);
    }
}