using Microsoft.Azure.Cosmos;

namespace Rystem.Azure.Integration.Cosmos
{
    public sealed record CosmosObjectConfiguration(ThroughputProperties Throughput, RequestOptions RequestOptions = default);
}