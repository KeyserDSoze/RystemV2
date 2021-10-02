using Azure.Identity;
using Azure.Storage.Queues;
using Rystem.Concurrency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Storage
{
    public sealed class QueueStorageIntegration : BaseStorageClient, IWarmUp
    {
        private QueueClient Context;
        private readonly string RaceId = Guid.NewGuid().ToString("N");
        public QueueStorageConfiguration Configuration { get; }
        public QueueStorageIntegration(QueueStorageConfiguration configuration, StorageAccount options) : base(options)
            => Configuration = configuration;
        private async Task<QueueClient> GetContextAsync()
        {
            if (Context == default)
                await RaceCondition.RunAsync(async () =>
                {
                    if (Context == default)
                    {
                        QueueClient queueClient = default;
                        if (!string.IsNullOrWhiteSpace(Account.AccountKey) || Account.UseKeyVault)
                        {
                            var client = Account.QueueClientOptions != default ?
                                new QueueServiceClient(await (Account as IRystemOptions).GetConnectionStringAsync().NoContext(), Account.QueueClientOptions) :
                                new QueueServiceClient(await (Account as IRystemOptions).GetConnectionStringAsync().NoContext());
                            queueClient = client.GetQueueClient(Configuration.Name.ToLower());
                        }
                        else
                        {
                            queueClient = Account.QueueClientOptions != default ?
                                new QueueClient(new Uri(string.Format("https://{0}.queue.core.windows.net/{1}",
                                                    Account.AccountName,
                                                    Configuration.Name.ToLower())),
                                                    new DefaultAzureCredential(), Account.QueueClientOptions) :
                                new QueueClient(new Uri(string.Format("https://{0}.queue.core.windows.net/{1}",
                                                    Account.AccountName,
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
            var client = Context ?? await GetContextAsync().NoContext();
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
            var client = Context ?? await GetContextAsync().NoContext();
            return !string.IsNullOrWhiteSpace((await client.SendMessageAsync(message).NoContext()).Value.MessageId);
        }

        public async Task<long> SendScheduledAsync(string message, int delayInSeconds)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            _ = await client.SendMessageAsync(message, new TimeSpan(0, 0, delayInSeconds)).NoContext();
            return 0;
        }

        public async Task<bool> WarmUpAsync()
        {
            await GetContextAsync().NoContext();
            return true;
        }
    }
}