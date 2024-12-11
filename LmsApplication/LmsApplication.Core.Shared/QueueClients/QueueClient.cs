using Azure.Storage.Queues;
using Newtonsoft.Json;

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
      var json = JsonConvert.SerializeObject(message);
      var bytes = System.Text.Encoding.UTF8.GetBytes(json);
      var base64 = Convert.ToBase64String(bytes);
      await _queueClient.SendMessageAsync(base64);
   }
}