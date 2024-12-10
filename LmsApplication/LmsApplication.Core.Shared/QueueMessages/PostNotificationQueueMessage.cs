using System.Text.Json.Serialization;
using LmsApplication.Core.Shared.Models;

namespace LmsApplication.Core.Shared.QueueMessages;

public class PostNotificationQueueMessage
{
    [JsonIgnore]
    public const string QueueName = "post-notification-queue";
    
    public required UserExchangeModel User { get; set; }
    
    public required UserExchangeModel Poster { get; set; }
    
    public required string CourseEditionName { get; set; }
    
    public required string CourseEditionId { get; set; }
    
    public required string PostBody { get; set; }
    
    public required DateTime TimeStampUtc { get; set; }
}