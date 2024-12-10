using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class GradeNotificationFunction
{
    private readonly ILogger<GradeNotificationFunction> _logger;
    private readonly IEmailService _emailService;

    public GradeNotificationFunction(ILogger<GradeNotificationFunction> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function(nameof(GradeNotificationFunction))]
    public async Task Run([QueueTrigger(GradeNotificationQueueMessage.QueueName, Connection = "StorageConnection")] GradeNotificationQueueMessage message)
    {
        _logger.LogInformation($"C# Queue trigger function processed: {message.TimeStampUtc}");
        
        await _emailService.CreateEmailAsync(message);
    }
}