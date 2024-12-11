using LmsApplication.Core.Shared.Models;
using Newtonsoft.Json;

namespace LmsApplication.Core.Shared.QueueMessages;

public class PostBatchNotificationQueueMessage
{
    [JsonIgnore]
    public const string QueueName = "post-batch-notification-queue";
    
    public required UserExchangeModel Poster { get; set; }
    
    public required string CourseEditionName { get; set; }
    
    public required Guid CourseEditionId { get; set; }
    
    public required string PostBody { get; set; }
    
    public required DateTime TimeStampUtc { get; set; }
}