using System.Text.Json;
using Azure.Storage.Queues;

namespace LmsApplication.Core.Shared.QueueClients;

public interface IQueueClient<T>
{
   Task EnqueueAsync(T message);
}

public class QueueClient<T> : IQueueClient<T>
{
   private readonly QueueClient _queueClient;

   public QueueClient(string connectionString, string queueName)
   {
      _queueClient = new QueueClient(connectionString, queueName);
   }

   public async Task EnqueueAsync(T message)
   {
      await _queueClient.SendMessageAsync(JsonSerializer.Serialize(message));
   }
}