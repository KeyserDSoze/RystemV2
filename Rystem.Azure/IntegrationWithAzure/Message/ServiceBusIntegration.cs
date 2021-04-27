using Azure.Identity;
using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.IntegrationWithAzure.Message
{
#warning AR - Missing dead lettering and defer
    /// <summary>
    /// https://github.com/Azure/azure-sdk-for-net/blob/Azure.Messaging.ServiceBus_7.1.2/sdk/servicebus/Azure.Messaging.ServiceBus/README.md
    /// </summary>
    internal class ServiceBusIntegration
    {
        private readonly ServiceBusSender Client;
        private readonly ServiceBusProcessor ClientReader;
        /// <summary>
        /// for instance something.servicebus.windows.net
        /// </summary>
        /// <param name="fullyQualifiedName"></param>
        /// <param name="queueName"></param>
        public ServiceBusIntegration(string fullyQualifiedName, string queueName, ServiceBusProcessorOptions options, bool hasDefaultCredential)
        {
            ServiceBusClient client = new(fullyQualifiedName, new DefaultAzureCredential());
            this.Client = client.CreateSender(queueName);
            this.ClientReader = client.CreateProcessor(queueName, options);
        }

        public ServiceBusIntegration(string connectionString, string queueName, ServiceBusProcessorOptions options)
        {
            ServiceBusClient client = new(connectionString);
            this.Client = client.CreateSender(queueName);
            this.ClientReader = client.CreateProcessor(queueName, options);
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