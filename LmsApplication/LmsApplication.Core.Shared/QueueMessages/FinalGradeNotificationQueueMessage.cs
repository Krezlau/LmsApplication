using LmsApplication.Core.Shared.Models;
using Newtonsoft.Json;

namespace LmsApplication.Core.Shared.QueueMessages;

public class FinalGradeNotificationQueueMessage
{
    [JsonIgnore]
    public const string QueueName = "final-grade-notification-queue";
    
    public required UserExchangeModel User { get; set; }
    
    public required UserExchangeModel Teacher { get; set; }
    
    public required string CourseEditionName { get; set; }
    
    public required Guid CourseEditionId { get; set; }
    
    public required decimal Grade { get; set; }
    
    public required DateTime TimeStampUtc { get; set; }
}