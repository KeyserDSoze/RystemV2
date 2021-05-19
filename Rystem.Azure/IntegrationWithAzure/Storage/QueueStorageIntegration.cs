using Azure.Identity;
using Azure.Storage.Queues;
using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Storage
{
    public sealed record QueueStorageConfiguration(string Name) : Configuration(Name)
    {
        public QueueStorageConfiguration() : this(string.Empty) { }
    }
    public sealed class QueueStorageIntegration : BaseStorageClient
    {
        private QueueClient Context;
        private readonly string RaceId = Guid.NewGuid().ToString("N");
        public QueueStorageConfiguration Configuration { get; }
        public QueueStorageIntegration(QueueStorageConfiguration configuration, StorageOptions options) : base(options)
            => Configuration = configuration;
        private async Task<QueueClient> GetContextAsync()
        {
            if (Context == default)
                await RaceCondition.RunAsync(async () =>
                {
                    if (Context == default)
                    {
                        QueueClient queueClient = default;
                        if (!string.IsNullOrWhiteSpace(Options.AccountKey))
                        {
                            var client = new QueueServiceClient(Options.GetConnectionString());
                            queueClient = client.GetQueueClient(Configuration.Name.ToLower());
                        }
                        else
                        {
                            queueClient = new QueueClient(new Uri(string.Format("https://{0}.queue.core.windows.net/{1}",
                                                Options.AccountName,
                                                Configuration.Name.ToLower())),
                                                new DefaultAzureCredential());
                        }
                        Context = queueClient;
                        if (!await queueClient.ExistsAsync().NoContext())
                            await queueClient.CreateIfNotExistsAsync().NoContext();
                    }
                }, RaceId).NoContext();
            return Context;
        }
        public async Task<List<(string Message, object Raw)>> ReadAsync(int max = 10)
        {
            var client = Context ?? await GetContextAsync();
            var messages = (await client.ReceiveMessagesAsync(max).NoContext()).Value;
            List<(string, object)> entities = new();
            foreach (var message in messages)
            {
                await client.DeleteMessageAsync(message.MessageId, message.PopReceipt).NoContext();
                entities.Add((message.MessageText, message));
            }
            return entities;
        }

        public async Task<bool> SendAsync(string message)
        {
            var client = Context ?? await GetContextAsync();
            return !string.IsNullOrWhiteSpace((await client.SendMessageAsync(message).NoContext()).Value.MessageId);
        }

        public async Task<long> SendScheduledAsync(string message, int delayInSeconds)
        {
            var client = Context ?? await GetContextAsync();
            _ = await client.SendMessageAsync(message, new TimeSpan(0, 0, delayInSeconds)).NoContext();
            return 0;
        }
    }
}
