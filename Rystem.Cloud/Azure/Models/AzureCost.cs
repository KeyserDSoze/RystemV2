using Rystem.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystem.Cloud.Azure
{
    internal sealed class AzureCost
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("location")]
        public object Location { get; set; }
        [JsonPropertyName("sku")]
        public object Sku { get; set; }
        [JsonPropertyName("eTag")]
        public object ETag { get; set; }
        [JsonPropertyName("properties")]
        public AzureCostProperties Properties { get; set; }
    }
    internal sealed class AzureCostProperties
    {
        [JsonPropertyName("nextLink")]
        public object NextLink { get; set; }
        [JsonPropertyName("columns")]
        public AzureColumn[] Columns { get; set; }
        [JsonPropertyName("rows")]
        public List<JsonElement> RawRows { get; set; }
        [JsonIgnore]
        public IEnumerable<List<JsonElement>> Rows => RawRows.Select(x => x.GetRawText().FromJson<List<JsonElement>>());
    }
    internal sealed class AzureColumn
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}