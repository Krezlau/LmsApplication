using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class PostNotificationFunction
{
    private readonly ILogger<PostNotificationFunction> _logger;
    private readonly IEmailCreateService _emailCreateService;

    public PostNotificationFunction(ILogger<PostNotificationFunction> logger, IEmailCreateService emailCreateService)
    {
        _logger = logger;
        _emailCreateService = emailCreateService;
    }

    [Function(nameof(PostNotificationFunction))]
    public void Run([QueueTrigger(PostNotificationQueueMessage.QueueName, Connection = "StorageConnection")] PostNotificationQueueMessage message)
    {
        _logger.LogInformation($"C# Queue trigger function processed: {message.TimeStampUtc}");
        
        _emailCreateService.CreateEmailAsync(message);
    }
}