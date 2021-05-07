using Azure.Identity;
using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Message
{
    //for instance something.servicebus.windows.net
    /// <summary>
    /// Leave ConnectionString empty if you want to connect through the managed identity
    /// </summary>
    public sealed record ServiceBusOptions(string FullyQualifiedName, string ConnectionString, ServiceBusProcessorOptions Options = null);
    public sealed record ServiceBusConfiguration(string QueueName);
#warning AR - Missing dead lettering and defer
    /// <summary>
    /// https://github.com/Azure/azure-sdk-for-net/blob/Azure.Messaging.ServiceBus_7.1.2/sdk/servicebus/Azure.Messaging.ServiceBus/README.md
    /// </summary>
    public sealed class ServiceBusIntegration
    {
        private readonly ServiceBusSender Client;
        private readonly ServiceBusProcessor ClientReader;

        public ServiceBusIntegration(ServiceBusConfiguration configuration, ServiceBusOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                ServiceBusClient client = new(options.ConnectionString);
                this.Client = client.CreateSender(configuration.QueueName);
                if (options.Options != null)
                    this.ClientReader = client.CreateProcessor(configuration.QueueName, options.Options);
            }
            else
            {
                ServiceBusClient client = new(options.FullyQualifiedName, new DefaultAzureCredential());
                this.Client = client.CreateSender(configuration.QueueName);
                if (options.Options != null)
                    this.ClientReader = client.CreateProcessor(configuration.QueueName, options.Options);
            }
        }

        public async Task StartReadAsync(Func<ProcessMessageEventArgs, Task> onMessage, Func<ProcessErrorEventArgs, Task> onError, CancellationToken cancellationToken = default)
        {
            ClientReader.ProcessMessageAsync += onMessage;
            ClientReader.ProcessErrorAsync += onError;
            await ClientReader.StartProcessingAsync(cancellationToken).NoContext();
        }
        public async Task StopReadAsync(CancellationToken cancellationToken = default)
            => await ClientReader.StopProcessingAsync(cancellationToken).NoContext();

        public async Task<long> SendAsync(string message, int delayInMilliseconds = 0, CancellationToken cancellationToken = default)
        {
            if (delayInMilliseconds <= 0)
            {
                await Client.SendMessageAsync(new ServiceBusMessage(message), cancellationToken).NoContext();
                return 0;
            }
            else
                return await Client.ScheduleMessageAsync(new ServiceBusMessage(message), DateTime.UtcNow.AddMilliseconds(delayInMilliseconds), cancellationToken).NoContext();
        }
        public async Task SendBatchAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default)
        {
            var batch = await Client.CreateMessageBatchAsync(cancellationToken).NoContext();
            foreach (var message in messages)
                batch.TryAddMessage(new ServiceBusMessage(message));
            await Client.SendMessagesAsync(batch, cancellationToken).NoContext();
        }
        public async Task RemoveScheduledAsync(long messageId, CancellationToken cancellationToken = default)
        {
            await Client.CancelScheduledMessageAsync(messageId, cancellationToken).NoContext();
        }
    }
}