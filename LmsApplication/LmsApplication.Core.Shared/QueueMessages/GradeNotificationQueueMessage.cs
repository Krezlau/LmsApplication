using System.Text.Json.Serialization;
using LmsApplication.Core.Shared.Models;

namespace LmsApplication.Core.Shared.QueueMessages;

public class GradeNotificationQueueMessage
{
    [JsonIgnore]
    public const string QueueName = "grade-notification-queue";
    
    public required UserExchangeModel User { get; set; }
    
    public required UserExchangeModel Teacher { get; set; }
    
    public required string CourseEditionName { get; set; }
    
    public required Guid CourseEditionId { get; set; }
    
    public required string Grade { get; set; }
    
    public required RowType RowType { get; set; }
    
    public required string RowName { get; set; }
    
    public required string? TeacherComment { get; set; }
    
    public required DateTime TimeStampUtc { get; set; }
}

public enum RowType
{
    Text,
    Number,
    Bool,
}