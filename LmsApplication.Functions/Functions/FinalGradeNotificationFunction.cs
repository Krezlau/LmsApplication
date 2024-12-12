using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class FinalGradeNotificationFunction
{
    private readonly ILogger<FinalGradeNotificationFunction> _logger;
    private readonly IEmailService _emailService;

    public FinalGradeNotificationFunction(ILogger<FinalGradeNotificationFunction> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function(nameof(FinalGradeNotificationFunction))]
    public async Task Run([QueueTrigger(FinalGradeNotificationQueueMessage.QueueName, Connection = "StorageConnection")] FinalGradeNotificationQueueMessage message)
    {
        _logger.LogInformation($"C# Queue trigger function processed: {message.TimeStampUtc}");
        
        await _emailService.CreateEmailAsync(message);
    }
}