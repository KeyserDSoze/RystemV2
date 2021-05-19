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
    public sealed record EventHubOptions(string FullyQualifiedName, string AccessKey, StorageOptions StorageOptions = default, EventProcessorClientOptions ProcessorOptions = default)
    {
        public string ConnectionString => $"Endpoint=sb://{FullyQualifiedName}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={AccessKey}";
    }
    public sealed record EventHubConfiguration(string Name, string ConsumerGroup = default) : Configuration(Name)
    {
        public EventHubConfiguration() : this(string.Empty) { }
    }
    public sealed class EventHubIntegration
    {
        private readonly EventHubProducerClient Client;
        private readonly EventProcessorClient ClientReader;
        public EventHubConfiguration Configuration { get; }
        private readonly BlobContainerClient BlobContainerClient;
        public EventHubIntegration(EventHubConfiguration configuration, EventHubOptions options)
        {
            Configuration = configuration;
            if (string.IsNullOrWhiteSpace(options.AccessKey))
                this.Client = new EventHubProducerClient(options.FullyQualifiedName, configuration.Name, new DefaultAzureCredential());
            else
                this.Client = new EventHubProducerClient(options.ConnectionString, configuration.Name);

            if (options.StorageOptions != default)
            {
                string consumerGroup = configuration.ConsumerGroup ?? "$Default";
                var containerName = $"{configuration.Name.ToLower()}consumergroup";
                BlobContainerClient = string.IsNullOrWhiteSpace(options.StorageOptions.AccountKey) ? new BlobContainerClient(new Uri(string.Format("https://{0}.blob.core.windows.net/{1}",
                                                options.StorageOptions.AccountName,
                                                containerName)),
                                                new DefaultAzureCredential()) :
                                                new BlobContainerClient(options.StorageOptions.GetConnectionString(), containerName);
                if (string.IsNullOrWhiteSpace(options.AccessKey))
                    ClientReader = new EventProcessorClient(BlobContainerClient, consumerGroup, options.FullyQualifiedName, configuration.Name, new DefaultAzureCredential(), options.ProcessorOptions);
                else
                    ClientReader = new EventProcessorClient(BlobContainerClient, consumerGroup, options.ConnectionString, configuration.Name, options.ProcessorOptions);
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