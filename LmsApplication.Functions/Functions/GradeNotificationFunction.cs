using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class GradeNotificationFunction
{
    private readonly ILogger<GradeNotificationFunction> _logger;
    private readonly IEmailCreateService _emailCreateService;

    public GradeNotificationFunction(ILogger<GradeNotificationFunction> logger, IEmailCreateService emailCreateService)
    {
        _logger = logger;
        _emailCreateService = emailCreateService;
    }

    [Function(nameof(GradeNotificationFunction))]
    public void Run([QueueTrigger(GradeNotificationQueueMessage.QueueName, Connection = "StorageConnection")] GradeNotificationQueueMessage message)
    {
        _logger.LogInformation($"C# Queue trigger function processed: {message.TimeStampUtc}");
        
        _emailCreateService.CreateEmailAsync(message);
    }
}