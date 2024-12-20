using System.Threading.Channels;
using LmsApplication.Core.Shared.Models;
using LmsApplication.CourseBoardModule.Data.Entities;

namespace LmsApplication.CourseBoardModule.Services.BackgroundServices;

public interface ISendPostNotificationsTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(SendPostNotificationWorkItem workItem);

    ValueTask<SendPostNotificationWorkItem> DequeueAsync(CancellationToken cancellationToken);
}

public class SendPostNotificationsTaskQueue : ISendPostNotificationsTaskQueue
{
    private readonly Channel<SendPostNotificationWorkItem> _queue;

    public SendPostNotificationsTaskQueue(int capacity)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<SendPostNotificationWorkItem>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(SendPostNotificationWorkItem workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<SendPostNotificationWorkItem> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}

public record SendPostNotificationWorkItem(Post Post, UserExchangeModel Poster, Guid EditionId);