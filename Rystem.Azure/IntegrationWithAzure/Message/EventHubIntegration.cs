using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.IntegrationWithAzure.Message
{
    internal class EventHubIntegration
    {
        private readonly EventHubProducerClient Client;
        private readonly EventProcessorClient ClientReader;
        /// <summary>
        /// for instance something.servicebus.windows.net
        /// </summary>
        /// <param name="fullyQualifiedName"></param>
        /// <param name="eventHubName"></param>
        public EventHubIntegration(string fullyQualifiedName, string eventHubName, bool hasDefaultCredential)
            => this.Client = new EventHubProducerClient(fullyQualifiedName, eventHubName, hasDefaultCredential ? new DefaultAzureCredential() : default);
        public EventHubIntegration(string connectionString, string eventHubName)
            => this.Client = new EventHubProducerClient(connectionString, eventHubName);
        public EventHubIntegration(string fullyQualifiedName, string eventHubName, string consumerGroup, string accountName, EventProcessorClientOptions options, string blobContainerName, bool hasDefaultCredential)
            : this(fullyQualifiedName, eventHubName, hasDefaultCredential)
        {
            var storageClient = new BlobContainerClient(new Uri(string.Format("https://{0}.blob.core.windows.net/{1}",
                                                accountName,
                                                blobContainerName)),
                                                new DefaultAzureCredential());
            ClientReader = new EventProcessorClient(storageClient, consumerGroup, fullyQualifiedName, eventHubName, new DefaultAzureCredential(), options);
        }
        public EventHubIntegration(string connectionString, string eventHubName, string consumerGroup, EventProcessorClientOptions options, string storageConnectionString, string blobContainerName)
            : this(connectionString, eventHubName)
        {
            var storageClient = new BlobContainerClient(storageConnectionString, blobContainerName);
            ClientReader = new EventProcessorClient(storageClient, consumerGroup, connectionString, eventHubName, options);
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