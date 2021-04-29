using Azure.Identity;
using Azure.Storage.Queues;
using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Azure.IntegrationWithAzure.Storage
{
    internal class QueueStorageIntegration : BaseStorageClient
    {
        private QueueClient Context;
        private readonly string RaceId = Guid.NewGuid().ToString("N");
        private readonly string QueueName;
        public QueueStorageIntegration(string queueName, StorageOptions options) : base(options)
        {
            this.QueueName = queueName;
        }
        private async Task<QueueClient> GetContextAsync()
        {
            if (Context == null)
                await RaceCondition.RunAsync(async () =>
                {
                    if (Context == null)
                    {
                        QueueClient queueClient = default;
                        if (!string.IsNullOrWhiteSpace(Options.ConnectionString))
                        {
                            var client = new QueueServiceClient(Options.ConnectionString);
                            queueClient = client.GetQueueClient(QueueName);
                        }
                        else
                        {
                            queueClient = new QueueClient(new Uri(string.Format("https://{0}.queue.core.windows.net/{1}",
                                                Options.AccountName,
                                                QueueName)),
                                                new DefaultAzureCredential());
                        }
                        if (!await queueClient.ExistsAsync().NoContext())
                            await queueClient.CreateIfNotExistsAsync().NoContext();
                        Context = queueClient;
                    }
                }, RaceId).NoContext();
            return Context;
        }
        public async Task<List<string>> ReadAsync(int max = 10)
        {
            var client = Context ?? await GetContextAsync();
            var messages = (await client.ReceiveMessagesAsync(max).NoContext()).Value;
            List<string> entities = new();
            foreach (var message in messages)
            {
                await client.DeleteMessageAsync(message.MessageId, message.PopReceipt).NoContext();
                entities.Add(message.MessageText);
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
