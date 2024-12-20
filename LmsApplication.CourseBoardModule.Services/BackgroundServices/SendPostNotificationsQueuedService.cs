using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.CourseBoardModule.Services.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LmsApplication.CourseBoardModule.Services.BackgroundServices;

public class SendPostNotificationsQueuedService : BackgroundService
{
    private readonly ILogger<SendPostNotificationsQueuedService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IQueueClient<PostNotificationQueueMessage> _postNotificationQueueClient;

    public SendPostNotificationsQueuedService(
        ISendPostNotificationsTaskQueue taskQueue,
        ILogger<SendPostNotificationsQueuedService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IQueueClient<PostNotificationQueueMessage> postNotificationQueueClient)
    {
        TaskQueue = taskQueue;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _postNotificationQueueClient = postNotificationQueueClient;
    }

    public ISendPostNotificationsTaskQueue TaskQueue { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await BackgroundProcessing(stoppingToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await TaskQueue.DequeueAsync(stoppingToken);

            try
            {
                await SendPostNotificationsAsync(workItem);
                _logger.LogInformation("Post notifications sent for post {PostId}.", workItem.Post.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
    
    private async Task SendPostNotificationsAsync(SendPostNotificationWorkItem workItem)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        
        var userProvider = scope.ServiceProvider.GetRequiredService<IUserProvider>();
        var courseEditionProvider = scope.ServiceProvider.GetRequiredService<ICourseEditionProvider>();
        
        var courseEdition = await courseEditionProvider.GetCourseEditionAsync(workItem.EditionId);
        if (courseEdition is null)
            return;
        
        var studentIds = await courseEditionProvider.GetCourseEditionStudentsAsync(workItem.EditionId);
        var participants = await userProvider.GetUsersByIdsAsync(studentIds);

        var timeStamp = DateTime.UtcNow;
        foreach (var participant in participants.Values.Where(x => x.Id != workItem.Poster.Id))
        {
            var postNotificationQueueMessage = new PostNotificationQueueMessage
            {
                User = participant,
                Poster = workItem.Poster,
                CourseEditionName = courseEdition.Name,
                CourseEditionId = courseEdition.Id,
                PostBody = workItem.Post.Content,
                TimeStampUtc = timeStamp,
            };
            
            await _postNotificationQueueClient.EnqueueAsync(postNotificationQueueMessage);
        }

    }
}
