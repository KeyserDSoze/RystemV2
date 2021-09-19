using Microsoft.Azure.Cosmos;

namespace Rystem.Azure.Integration.Cosmos
{
    public sealed record CosmosConfiguration(ContainerProperties ContainerProperties = default, CosmosObjectConfiguration Database = default, CosmosObjectConfiguration Container = default, CosmosClientOptions ClientOptions = default, string DatabaseName = default) : Configuration(ContainerProperties?.Id ?? string.Empty)
    {
        public CosmosConfiguration() : this(default) { }
    }
}