using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LmsApplication.Functions.Functions;

public class PostBatchNotificationFunction
{
    private readonly ILogger<PostBatchNotificationFunction> _logger;
    private readonly ICourseEditionParticipantsProviderService _courseEditionParticipantsProviderService;
    private readonly IQueueClient<PostNotificationQueueMessage> _postNotificationQueueClient;

    public PostBatchNotificationFunction(
        ILogger<PostBatchNotificationFunction> logger,
        ICourseEditionParticipantsProviderService courseEditionParticipantsProviderService,
        IQueueClient<PostNotificationQueueMessage> postNotificationQueueClient)
    {
        _logger = logger;
        _courseEditionParticipantsProviderService = courseEditionParticipantsProviderService;
        _postNotificationQueueClient = postNotificationQueueClient;
    }

    [Function(nameof(PostBatchNotificationFunction))]
    public async Task Run([QueueTrigger(PostBatchNotificationQueueMessage.QueueName, Connection = "StorageConnection")] PostBatchNotificationQueueMessage message)
    {
        _logger.LogInformation($"C# Queue trigger function processed: {message.TimeStampUtc}");
        
        var courseEditionParticipants = await _courseEditionParticipantsProviderService.GetCourseEditionParticipantsAsync(message.CourseEditionId);
        
        foreach (var participant in courseEditionParticipants)
        {
            var postNotificationQueueMessage = new PostNotificationQueueMessage
            {
                User = participant,
                Poster = message.Poster,
                CourseEditionName = message.CourseEditionName,
                CourseEditionId = message.CourseEditionId,
                PostBody = message.PostBody,
                TimeStampUtc = message.TimeStampUtc
            };
            
            await _postNotificationQueueClient.EnqueueAsync(postNotificationQueueMessage);
        }
    }
}