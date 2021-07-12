using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.Cloud.Azure
{
    internal sealed class AzureMonitoring
    {
        [JsonPropertyName("cost")]
        public int Cost { get; set; }
        [JsonPropertyName("timespan")]
        public string Timespan { get; set; }
        [JsonPropertyName("interval")]
        public string Interval { get; set; }
        [JsonPropertyName("value")]
        public AzureMonitoringValue[] Value { get; set; }
        [JsonPropertyName("_namespace")]
        public string _Namespace { get; set; }
        [JsonPropertyName("resourceregion")]
        public string Resourceregion { get; set; }
    }

    internal sealed class AzureMonitoringValue
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("name")]
        public AzureMonitoringName Name { get; set; }
        [JsonPropertyName("displayDescription")]
        public string DisplayDescription { get; set; }
        [JsonPropertyName("unit")]
        public string Unit { get; set; }
        [JsonPropertyName("timeseries")]
        public List<AzureMonitoringTimesery> Timeseries { get; set; }
        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }
    }

    internal sealed class AzureMonitoringName
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("localizedValue")]
        public string LocalizedValue { get; set; }
    }

    internal sealed class AzureMonitoringTimesery
    {
        [JsonPropertyName("metadatavalues")]
        public object[] MetadataValues { get; set; }
        [JsonPropertyName("data")]
        public List<AzureDatum> Data { get; set; }
    }
    internal sealed class AzureDatum
    {
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
        [JsonPropertyName("average")]
        public double Average { get; set; } 
        [JsonPropertyName("maximum")]
        public double Maximum { get; set; }
    }
}