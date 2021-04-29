using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using Rystem.Azure.IntegrationWithAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.IntegrationWithAzure.Message
{
    /// <summary>
    /// Leave ConnectionString empty if you want to connect through the managed identity
    /// </summary>
    public class EventHubOptions
    {
        public string FullyQualifiedName { get; init; }
        public string ConnectionString { get; init; }
        public EventProcessorClientOptions Options { get; init; }
        public StorageOptions StorageOptions { get; init; }
    }
    internal class EventHubIntegration
    {
        private readonly EventHubProducerClient Client;
        private readonly EventProcessorClient ClientReader;
        public EventHubIntegration(string eventHubName, EventHubOptions options, string consumerGroup = null, EventProcessorClientOptions processorOptions = null)
        {
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                this.Client = new EventHubProducerClient(options.FullyQualifiedName, eventHubName, new DefaultAzureCredential());
            else
                this.Client = new EventHubProducerClient(options.ConnectionString, eventHubName);

            if (!string.IsNullOrWhiteSpace(consumerGroup) && options.StorageOptions != null)
            {
                var containerName = $"{eventHubName.ToLower()}consumergroup";
                var storageClient = string.IsNullOrWhiteSpace(options.StorageOptions.AccountKey) ? new BlobContainerClient(new Uri(string.Format("https://{0}.blob.core.windows.net/{1}",
                                                options.StorageOptions.AccountName,
                                                containerName)),
                                                new DefaultAzureCredential()) :
                                                new BlobContainerClient(options.StorageOptions.ConnectionString, containerName);
                if (string.IsNullOrWhiteSpace(options.ConnectionString))
                    ClientReader = new EventProcessorClient(storageClient, consumerGroup, options.FullyQualifiedName, eventHubName, new DefaultAzureCredential(), processorOptions);
                else
                    ClientReader = new EventProcessorClient(storageClient, consumerGroup, options.ConnectionString, eventHubName, processorOptions);
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

        public async Task SendAsync(string message, string partitionKey = null, string partitionId = null, CancellationToken cancellationToken = default)
            => await SendBatchAsync(new string[1] { message }, partitionKey, partitionId, cancellationToken);
        public async Task SendBatchAsync(IEnumerable<string> messages, string partitionKey = null, string partitionId = null, CancellationToken cancellationToken = default)
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