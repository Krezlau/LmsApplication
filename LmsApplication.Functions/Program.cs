using System.Net;
using System.Net.Mail;
using LmsApplication.Core.Shared.QueueClients;
using LmsApplication.Core.Shared.QueueMessages;
using LmsApplication.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services)=>
    {
        var azureStorage = context.Configuration.GetValue<string>("StorageConnection");
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailCreateService, EmailCreateService>();
        services.AddScoped<SmtpClient>(_ => new SmtpClient
        {
            Host = context.Configuration.GetValue<string>("SmtpServer"),
            Port = context.Configuration.GetValue<int>("SmtpPort"),
            Credentials = new NetworkCredential(context.Configuration.GetValue<string>("SmtpUsername"),
                context.Configuration.GetValue<string>("SmtpPassword")),
            EnableSsl = true
        });
        services.AddSingleton<IQueueClient<NotificationQueueMessage>>(_ =>
            new QueueClient<NotificationQueueMessage>(azureStorage, NotificationQueueMessage.QueueName));
    })
    .Build();

host.Run();