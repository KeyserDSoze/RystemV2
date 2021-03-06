using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Rystem.Concurrency;
using Rystem.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Storage
{
    public sealed class BlobStorageIntegration : BaseStorageClient, IWarmUp
    {
        private BlobContainerClient Context;
        private readonly string RaceId = Guid.NewGuid().ToString("N");
        private readonly string LockRaceId = Guid.NewGuid().ToString("N");
        public BlobStorageConfiguration Configuration { get; }
        public BlobStorageIntegration(BlobStorageConfiguration configuration, StorageAccount account) : base(account)
            => Configuration = configuration;
        private async Task<BlobContainerClient> GetContextAsync()
        {
            if (Context == default)
                await RaceCondition.RunAsync(async () =>
                {
                    if (Context == default)
                    {
                        BlobContainerClient blobClient = default;
                        if (!string.IsNullOrWhiteSpace(Account.AccountKey) || Account.UseKeyVault)
                        {
                            var client =
                                Account.BlobClientOptions != default ?
                                    new BlobServiceClient(await (Account as IRystemOptions).GetConnectionStringAsync().NoContext(), Account.BlobClientOptions) :
                                    new BlobServiceClient(await (Account as IRystemOptions).GetConnectionStringAsync().NoContext());
                            blobClient = client.GetBlobContainerClient(Configuration.Name.ToLower());
                        }
                        else
                        {
                            blobClient = Account.BlobClientOptions != default ?
                                        new BlobContainerClient(new Uri(string.Format("https://{0}.blob.core.windows.net/{1}",
                                            Account.AccountName,
                                            Configuration.Name.ToLower())),
                                            new DefaultAzureCredential(), Account.BlobClientOptions) :
                                        new BlobContainerClient(new Uri(string.Format("https://{0}.blob.core.windows.net/{1}",
                                            Account.AccountName,
                                            Configuration.Name.ToLower())),
                                            new DefaultAzureCredential());
                        }
                        Context = blobClient;
                        if (!await blobClient.ExistsAsync().NoContext())
                            await blobClient.CreateIfNotExistsAsync().NoContext();
                    }
                }, RaceId).NoContext();
            return Context;
        }
        public async Task<bool> DeleteAsync(string name)
        {
            var client = Context ?? await GetContextAsync();
            return await client.GetBlobClient(name).DeleteIfExistsAsync().NoContext();
        }
        public async Task<bool> ExistsAsync(string name)
        {
            var client = Context ?? await GetContextAsync();
            return await client.GetBlobClient(name).ExistsAsync().NoContext();
        }
        public async Task<List<(string Uri, string Name)>> SearchAsync(string prefix = default, int? takeCount = default, CancellationToken token = default)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            List<(string Uri, string Name)> items = new();
            int count = 0;
            await foreach (var t in client.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, token))
            {
                items.Add(($"{client.Uri}/{t.Name}", t.Name));
                count++;
                if (takeCount != default && items.Count >= takeCount)
                    break;
            }
            return items;
        }
        public async Task<List<BlobPropertyWrapper>> FetchPropertiesAsync(string prefix = default, int? takeCount = default, CancellationToken token = default)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            List<BlobPropertyWrapper> items = new();
            int count = 0;
            await foreach (var blob in client.GetBlobsAsync(BlobTraits.All, BlobStates.All, prefix, token))
            {
                items.Add(new BlobPropertyWrapper() { Name = blob.Name, ItemProperties = blob.Properties, Tags = blob.Tags });
                count++;
                if (takeCount != default && items.Count >= takeCount)
                    break;
            }
            return items;
        }
        public async Task<bool> SetBlobPropertiesAsync(string name, BlobStorageProperties properties, CancellationToken token = default)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            var blobClient = client.GetBlobClient(name);
            if (properties.Headers != default)
                await blobClient.SetHttpHeadersAsync(properties.Headers, properties.HeadersConditions, token).NoContext();
            if (properties.Metadata != default)
                await blobClient.SetMetadataAsync(properties.Metadata, properties.MetadataConditions, token).NoContext();
            if (properties.Tags != default)
                await blobClient.SetTagsAsync(properties.Tags, properties.TagConditions, token).NoContext();
            return true;
        }
        public async Task<BlobWrapper> ReadAsync(string name, bool fetchProperties = false)
        {
            var client = Context ?? await GetContextAsync().NoContext();
            var blobClient = client.GetBlobClient(name);
            var memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream).NoContext();
            memoryStream.Position = 0;
            BlobProperties properties = default;
            IDictionary<string, string> tags = default;
            if (fetchProperties)
            {
                properties = (await blobClient.GetPropertiesAsync().NoContext()).Value;
                tags = (await blobClient.GetTagsAsync().NoContext()).Value.Tags;
            }
            return new BlobWrapper
            {
                Name = name,
                Content = memoryStream,
                Properties = properties,
                Tags = tags
            };
        }
        public async Task<BlobWrapper> ReadPropertiesAsync(string name)
        {
            var client = Context ?? await GetContextAsync();
            var blobClient = client.GetBlobClient(name);
            var properties = (await blobClient.GetPropertiesAsync().NoContext()).Value;
            var tags = (await blobClient.GetTagsAsync().NoContext()).Value.Tags;
            return new BlobWrapper() { Name = name, Properties = properties, Tags = tags };
        }
        public async Task<List<BlobWrapper>> ListAsync(string prefix = default, int? takeCount = default, CancellationToken token = default)
        {
            var client = Context ?? await GetContextAsync();
            List<BlobWrapper> items = new();
            int count = 0;
            await foreach (var blob in client.GetBlobsAsync(BlobTraits.All, BlobStates.All, prefix, token))
            {
                items.Add(new BlobWrapper() { Name = blob.Name, Content = (await ReadAsync(blob.Name, false).NoContext()).Content, ItemProperties = blob.Properties, Tags = blob.Tags });
                count++;
                if (takeCount != default && items.Count >= takeCount)
                    break;
            }
            return items;
        }
        public async Task<bool> WriteBlockAsync(string name, Stream stream, BlobUploadOptions options)
        {
            var client = Context ?? await GetContextAsync();
            BlockBlobClient cloudBlob = client.GetBlockBlobClient(name);
            stream.Position = 0;
            await cloudBlob.UploadAsync(stream, options ?? new()).NoContext();
            if (stream is NotClosableStream)
                (stream as NotClosableStream).ManualDispose();
            return true;
        }
        private const int MaximumAttemptForAppendWriting = 3;
        public async Task<bool> WriteAppendAsync(string name, Stream stream)
        {
            var client = Context ?? await GetContextAsync();
            int attempt = 0;
            AppendBlobClient appendBlob = client.GetAppendBlobClient(name);
            do
            {
                try
                {
                    stream.Position = 0;
                    await appendBlob.AppendBlockAsync(stream).NoContext();
                    break;
                }
                catch (AggregateException aggregateException)
                {
                    await Task.Delay(20).NoContext();
                    if (attempt >= MaximumAttemptForAppendWriting)
                        throw aggregateException;
                }
                catch (RequestFailedException er)
                {
                    if (er.Status == 404)
                        await appendBlob.CreateIfNotExistsAsync().NoContext();
                    else
                        throw er;
                }
                attempt++;
            } while (attempt <= MaximumAttemptForAppendWriting);
            if (stream is NotClosableStream)
                (stream as NotClosableStream).ManualDispose();
            return attempt <= MaximumAttemptForAppendWriting;
        }
        private const long PageSize = 512;
        public async Task<bool> WritePageAsync(string name, Stream stream, long offset)
        {
            var client = Context ?? await GetContextAsync();
            var pageBlob = client.GetPageBlobClient(name);
            if (!await pageBlob.ExistsAsync().NoContext())
                await pageBlob.CreateAsync(PageSize);
            stream.Position = 0;
            long sized = PageSize - stream.Length;
            if (sized != 0)
            {
                byte[] baseMemoryStream = new BinaryReader(stream).ReadBytes((int)stream.Length);
                byte[] finalizingStream = new byte[PageSize];
                for (int i = 0; i < PageSize; i++)
                    finalizingStream[i] = i < stream.Length ? baseMemoryStream[i] : (byte)0;
                await pageBlob.UploadPagesAsync(new MemoryStream(finalizingStream), PageSize * offset, null).NoContext();
            }
            if (stream is NotClosableStream)
                (stream as NotClosableStream).ManualDispose();
#warning È sicuramente buggato, perchè scrive solo quando è in un if buggato
            return true;
        }
        private readonly ConcurrentDictionary<string, BlobLeaseClient> TokenAcquireds = new();
        private static readonly MemoryStream EmptyStream = new(Array.Empty<byte>());
        private readonly Dictionary<string, BlobLockWrapper> BlobLockClients = new();
        private async Task<BlobLockWrapper> GetBlobClientForLockAsync(string name)
        {
            if (!BlobLockClients.ContainsKey(name))
            {
                await RaceCondition.RunAsync(async () =>
                {
                    if (!BlobLockClients.ContainsKey(name))
                    {
                        var client = Context ?? await GetContextAsync();
                        string officialName = $"_{name}.lock";
                        var preBlob = client.GetBlobClient(officialName);
                        if (!await preBlob.ExistsAsync().NoContext())
                            await preBlob.UploadAsync(EmptyStream).NoContext();
                        BlobLockClients.Add(name, new BlobLockWrapper
                        {
                            Client = preBlob
                        });
                    }
                }, $"{LockRaceId}{name}").NoContext();
            }
            return BlobLockClients[name];
        }
        public async Task<bool> AcquireLockAsync(string key)
        {
            try
            {
                string normalizedKey = key ?? string.Empty;
                if (!this.TokenAcquireds.ContainsKey(normalizedKey))
                {
                    var blob = await this.GetBlobClientForLockAsync(normalizedKey).NoContext();
                    RaceConditionResponse response = await RaceCondition.RunAsync(async () =>
                    {
                        var lease = blob.Client.GetBlobLeaseClient(BlobLockClients[normalizedKey].LeaseId);
                        Response<BlobLease> response = await lease.AcquireAsync(new TimeSpan(0, 1, 0)).NoContext();
                        this.TokenAcquireds.TryAdd(normalizedKey, lease);
                    }, BlobLockClients[normalizedKey].AcquireLockId).NoContext();
                    return response.IsExecuted && !response.InException;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> LockIsAcquiredAsync(string key)
        {
            string normalizedKey = key ?? string.Empty;
            if (this.TokenAcquireds.ContainsKey(normalizedKey))
                return true;
            var blob = await this.GetBlobClientForLockAsync(normalizedKey).NoContext();
            Response<BlobProperties> properties = await blob.Client.GetPropertiesAsync().NoContext();
            return properties.Value.LeaseStatus == LeaseStatus.Locked;
        }
        public async Task<bool> ReleaseLockAsync(string key)
        {
            string normalizedKey = key ?? string.Empty;
            if (TokenAcquireds.ContainsKey(normalizedKey))
                await RaceCondition.RunAsync(async () =>
                {
                    _ = await TokenAcquireds[normalizedKey].ReleaseAsync().NoContext();
                    TokenAcquireds.TryRemove(normalizedKey, out _);
                }, BlobLockClients[normalizedKey].ReleaseLockId).NoContext();
            return true;
        }

        public async Task<bool> WarmUpAsync()
        {
            await GetContextAsync().NoContext();
            return true;
        }

        public class BlobPropertyWrapper
        {
            public string Name { get; init; }
            public BlobItemProperties ItemProperties { get; init; }
            public IDictionary<string, string> Tags { get; init; }
        }
        public class BlobWrapper : BlobPropertyWrapper
        {
            public BlobProperties Properties { get; init; }
            public Stream Content { get; init; }
        }
        public class BlobLockWrapper
        {
            public BlobClient Client { get; set; }
            public string AcquireLockId { get; } = Guid.NewGuid().ToString("N");
            public string LeaseId { get; } = Guid.NewGuid().ToString("N");
            public string ReleaseLockId { get; } = Guid.NewGuid().ToString("N");
        }
    }
}