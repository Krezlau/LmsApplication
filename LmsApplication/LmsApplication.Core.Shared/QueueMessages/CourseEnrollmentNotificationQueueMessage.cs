using System.Text.Json.Serialization;
using LmsApplication.Core.Shared.Models;

namespace LmsApplication.Core.Shared.QueueMessages;

public class CourseEnrollmentNotificationQueueMessage
{
    [JsonIgnore]
    public const string QueueName = "course-enrollment-notification-queue";
    
    public required UserExchangeModel User { get; set; }
    
    public required string CourseEditionName { get; set; }
    
    public required string CourseEditionId { get; set; }
    
    public required string CourseName { get; set; }
    
    public required DateTime TimeStampUtc { get; set; }
}