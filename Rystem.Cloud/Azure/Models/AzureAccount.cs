using System;
using System.Text.Json.Serialization;

namespace Rystem.Cloud.Azure
{
    internal sealed class AzureAccount
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonPropertyName("ext_expires_in")]
        public long ExtExpiresIn { get; set; }
        [JsonPropertyName("expires_on")]
        public long ExpiresOn { get; set; }
        [JsonPropertyName("not_before")]
        public long NotBefore { get; set; }
        [JsonPropertyName("resource")]
        public string Resource { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        public DateTime StartTime { get; } = DateTime.UtcNow;
        public DateTime EndTime => StartTime.AddSeconds(ExpiresIn);
    }
}