using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using Rystem.Azure.Integration.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Message
{
    /// <summary>
    /// Leave ConnectionString empty if you want to connect through the managed identity
    /// </summary>
    public sealed record EventHubOptions(string FullyQualifiedName, string ConnectionString, EventProcessorClientOptions ProcessorOptions, StorageOptions StorageOptions);
    public sealed record EventHubConfiguration(string EventHubName, string ConsumerGroup = default);
    public sealed class EventHubIntegration
    {
        private readonly EventHubProducerClient Client;
        private readonly EventProcessorClient ClientReader;
        public EventHubIntegration(EventHubConfiguration configuration, EventHubOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                this.Client = new EventHubProducerClient(options.FullyQualifiedName, configuration.EventHubName, new DefaultAzureCredential());
            else
                this.Client = new EventHubProducerClient(options.ConnectionString, configuration.EventHubName);

            if (!string.IsNullOrWhiteSpace(configuration.ConsumerGroup) && options.StorageOptions != default)
            {
                var containerName = $"{configuration.EventHubName.ToLower()}consumergroup";
                var storageClient = string.IsNullOrWhiteSpace(options.StorageOptions.AccountKey) ? new BlobContainerClient(new Uri(string.Format("https://{0}.blob.core.windows.net/{1}",
                                                options.StorageOptions.AccountName,
                                                containerName)),
                                                new DefaultAzureCredential()) :
                                                new BlobContainerClient(options.StorageOptions.GetConnectionString(), containerName);
                if (string.IsNullOrWhiteSpace(options.ConnectionString))
                    ClientReader = new EventProcessorClient(storageClient, configuration.ConsumerGroup, options.FullyQualifiedName, configuration.EventHubName, new DefaultAzureCredential(), options.ProcessorOptions);
                else
                    ClientReader = new EventProcessorClient(storageClient, configuration.ConsumerGroup, options.ConnectionString, configuration.EventHubName, options.ProcessorOptions);
            }
        }
        public async Task<IEnumerable<string>> ListPartitionAsync()
            => await this.Client.GetPartitionIdsAsync().NoContext();

        public async Task StartReadAsync(Func<ProcessEventArgs, Task> onMessage, Func<ProcessErrorEventArgs, Task> onError, CancellationToken cancellationToken = default)
        {
            ClientReader.ProcessEventAsync += onMessage;
            ClientReader.ProcessErrorAsync += onError;
            await ClientReader.StartProcessingAsync(cancellationToken).NoContext();
        }
        public async Task StopReadAsync(CancellationToken cancellationToken = default)
            => await ClientReader.StopProcessingAsync(cancellationToken).NoContext();

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