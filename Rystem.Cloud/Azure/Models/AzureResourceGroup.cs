using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rystem.Cloud.Azure
{
    internal class AzureResourceGroups
    {
        [JsonPropertyName("value")]
        public List<AzureResourceGroup> ResourceGroups { get; set; }
    }

    internal sealed class AzureResourceGroup
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("properties")]
        public AzureProperties Properties { get; set; }
        [JsonPropertyName("tags")]
        public Dictionary<string, string> Tags { get; set; }
    }

    internal sealed class AzureProperties
    {
        [JsonPropertyName("provisioningState")]
        public string ProvisioningState { get; set; }
    }
}