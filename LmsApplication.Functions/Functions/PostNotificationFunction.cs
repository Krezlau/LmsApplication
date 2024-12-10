using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class PostNotificationFunction
{
    private readonly ILogger<PostNotificationFunction> _logger;
    private readonly IEmailService _emailService;

    public PostNotificationFunction(ILogger<PostNotificationFunction> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function(nameof(PostNotificationFunction))]
    public void Run([QueueTrigger(PostNotificationQueueMessage.QueueName, Connection = "StorageConnection")] PostNotificationQueueMessage message)
    {
        _logger.LogInformation($"C# Queue trigger function processed: {message.TimeStampUtc}");
        
        _emailService.CreateEmailAsync(message);
    }
}