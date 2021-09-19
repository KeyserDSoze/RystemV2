using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Message
{
    public sealed class EventHubIntegration
    {
        private readonly EventHubProducerClient Client;
        private readonly EventProcessorClient ClientReader;
        public EventHubConfiguration Configuration { get; }
        private readonly BlobContainerClient BlobContainerClient;
        public EventHubIntegration(EventHubConfiguration configuration, EventHubAccount options)
        {
            Configuration = configuration;
            if (string.IsNullOrWhiteSpace(options.AccessKey) && !options.UseKeyVault)
                this.Client = new EventHubProducerClient(options.FullyQualifiedName, configuration.Name, new DefaultAzureCredential());
            else
                this.Client = new EventHubProducerClient((options as IRystemOptions).GetConnectionStringAsync().ToResult(), configuration.Name);

            if (options.StorageOptions != default)
            {
                string consumerGroup = configuration.ConsumerGroup ?? "$Default";
                var containerName = $"{configuration.Name.ToLower()}consumergroup";
                BlobContainerClient = string.IsNullOrWhiteSpace(options.StorageOptions.AccountKey) && !options.StorageOptions.UseKeyVault ?
                    new BlobContainerClient(new Uri(string.Format("https://{0}.blob.core.windows.net/{1}",
                                                options.StorageOptions.AccountName,
                                                containerName)),
                                                new DefaultAzureCredential()) :
                    new BlobContainerClient((options.StorageOptions as IRystemOptions).GetConnectionStringAsync().ToResult(), containerName);
                if (string.IsNullOrWhiteSpace(options.AccessKey) && !options.UseKeyVault)
                    ClientReader = new EventProcessorClient(BlobContainerClient, consumerGroup, options.FullyQualifiedName, configuration.Name, new DefaultAzureCredential(), options.ProcessorOptions);
                else
                    ClientReader = new EventProcessorClient(BlobContainerClient, consumerGroup, (options as IRystemOptions).GetConnectionStringAsync().ToResult(), configuration.Name, options.ProcessorOptions);
            }
        }
        public async Task<IEnumerable<string>> ListPartitionAsync()
            => await this.Client.GetPartitionIdsAsync().NoContext();

        private Func<ProcessEventArgs, Task> OnMessage;
        private Func<ProcessErrorEventArgs, Task> OnError;
        public async Task StartReadAsync(Func<ProcessEventArgs, Task> onMessage, Func<ProcessErrorEventArgs, Task> onError, CancellationToken cancellationToken = default)
        {
            OnMessage = onMessage;
            OnError = onError;
            ClientReader.ProcessEventAsync += OnMessage;
            ClientReader.ProcessErrorAsync += OnError;
            await Task.WhenAll(BlobContainerClient.CreateIfNotExistsAsync(),
                ClientReader.StartProcessingAsync(cancellationToken)).NoContext();
        }
        public async Task StopReadAsync(CancellationToken cancellationToken = default)
        {
            await ClientReader.StopProcessingAsync(cancellationToken).NoContext();
            ClientReader.ProcessEventAsync -= OnMessage;
            ClientReader.ProcessErrorAsync -= OnError;
        }

        public async Task SendAsync(string message, string partitionKey = default, string partitionId = default, CancellationToken cancellationToken = default)
            => await SendBatchAsync(new string[1] { message }, partitionKey, partitionId, cancellationToken);
        public async Task SendBatchAsync(IEnumerable<string> messages, string partitionKey = default, string partitionId = default, CancellationToken cancellationToken = default)
        {
            await Client.SendAsync(
                messages.Select(x => new EventData(new BinaryData(x.ToByteArray()))),
                new SendEventOptions
                {
                    PartitionId = partitionId,
                    PartitionKey = partitionKey
                },
                cancellationToken).NoContext();
        }
    }
}