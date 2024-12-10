using System.Text.Json.Serialization;

namespace LmsApplication.Core.Shared.QueueMessages;

public class NotificationQueueMessage
{
    [JsonIgnore]
    public const string QueueName = "notifications";
    
    public required string Title { get; set; }
    
    public required string Recipient { get; set; }
    
    public required string Body { get; set; }
}