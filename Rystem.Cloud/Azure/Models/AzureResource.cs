using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.Cloud.Azure
{
    internal sealed class AzureResources
    {
        [JsonPropertyName("value")]
        public List<AzureResource> Resources { get; set; }
    }

    internal sealed class AzureResource
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("kind")]
        public string Kind { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("tags")]
        public Dictionary<string, string> Tags { get; set; }
        [JsonPropertyName("sku")]
        public AzureSku Sku { get; set; }
        [JsonPropertyName("managedBy")]
        public string ManagedBy { get; set; }
        [JsonPropertyName("plan")]
        public AzurePlan Plan { get; set; }
    }

    internal sealed class AzureSku
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("tier")]
        public string Tier { get; set; }
        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }
        [JsonPropertyName("size")]
        public string Size { get; set; }
        [JsonPropertyName("family")]
        public string Family { get; set; }
    }

    internal sealed class AzurePlan
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("promotionCode")]
        public string PromotionCode { get; set; }
        [JsonPropertyName("product")]
        public string Product { get; set; }
        [JsonPropertyName("publisher")]
        public string Publisher { get; set; }
    }
}