using Azure.Identity;
using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Message
{
#warning AR - Missing dead lettering and defer
    /// <summary>
    /// https://github.com/Azure/azure-sdk-for-net/blob/Azure.Messaging.ServiceBus_7.1.2/sdk/servicebus/Azure.Messaging.ServiceBus/README.md
    /// </summary>
    public sealed class ServiceBusIntegration : IWarmUp
    {
        private readonly ServiceBusSender Client;
        private readonly ServiceBusProcessor ClientReader;
        public ServiceBusConfiguration Configuration { get; }

        public ServiceBusIntegration(ServiceBusConfiguration configuration, ServiceBusAccount options)
        {
            Configuration = configuration;
            if (!string.IsNullOrWhiteSpace(options.AccessKey) || options.UseKeyVault)
            {
                ServiceBusClient client = new((options as IRystemOptions).GetConnectionStringAsync().ToResult());
                this.Client = client.CreateSender(configuration.Name);
                this.ClientReader = client.CreateProcessor(configuration.Name, options.Options);
            }
            else
            {
                ServiceBusClient client = new(options.FullyQualifiedName, new DefaultAzureCredential());
                this.Client = client.CreateSender(configuration.Name);
                this.ClientReader = client.CreateProcessor(configuration.Name, options.Options);
            }
        }

        private Func<ProcessMessageEventArgs, Task> OnMessage;
        private Func<ProcessErrorEventArgs, Task> OnError;
        public async Task StartReadAsync(Func<ProcessMessageEventArgs, Task> onMessage, Func<ProcessErrorEventArgs, Task> onError, CancellationToken cancellationToken = default)
        {
            OnMessage = onMessage;
            OnError = onError;
            ClientReader.ProcessMessageAsync += OnMessage;
            ClientReader.ProcessErrorAsync += OnError;
            await ClientReader.StartProcessingAsync(cancellationToken).NoContext();
        }
        public async Task StopReadAsync(CancellationToken cancellationToken = default)
        {
            await ClientReader.StopProcessingAsync(cancellationToken).NoContext();
            ClientReader.ProcessMessageAsync -= OnMessage;
            ClientReader.ProcessErrorAsync -= OnError;
        }

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

        public Task<bool> WarmUpAsync() 
            => Task.FromResult(true);
    }
}