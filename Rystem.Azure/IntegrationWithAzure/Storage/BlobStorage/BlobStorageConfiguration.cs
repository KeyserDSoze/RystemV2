using Azure.Storage.Blobs;

namespace Rystem.Azure.Integration.Storage
{
    public sealed record BlobStorageConfiguration(string Name) : Configuration(Name)
    {
        public BlobStorageConfiguration() : this(string.Empty) { }
    }
}