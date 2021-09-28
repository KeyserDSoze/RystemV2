using System;
using System.Text.Json.Serialization;

namespace Rystem.Cloud.Azure
{
    internal sealed class AzureConsumptions
    {
        [JsonPropertyName("value")]
        public AzureConsumption[] Value { get; set; }
        [JsonPropertyName("nextLink")]
        public string NextLink { get; set; }
    }

    internal sealed class AzureConsumption
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("tags")]
        public AzureConsumptionTags Tags { get; set; }
        [JsonPropertyName("properties")]
        public AzureConsumptionProperties Properties { get; set; }
    }

    internal sealed class AzureConsumptionTags
    {
        [JsonPropertyName("migration")]
        public string Migration { get; set; }
        [JsonPropertyName("msresourceusage")]
        public string MsResourceUsage { get; set; }
        [JsonPropertyName("environment")]
        public string Environment { get; set; }
        public string Test { get; set; }
    }

    internal sealed class AzureConsumptionProperties
    {
        [JsonPropertyName("billingAccountId")]
        public string BillingAccountId { get; set; }
        [JsonPropertyName("billingAccountName")]
        public string BillingAccountName { get; set; }
        [JsonPropertyName("billingPeriodStartDate")]
        public DateTime BillingPeriodStartDate { get; set; }
        [JsonPropertyName("billingPeriodEndDate")]
        public DateTime BillingPeriodEndDate { get; set; }
        [JsonPropertyName("billingProfileId")]
        public string BillingProfileId { get; set; }
        [JsonPropertyName("billingProfileName")]
        public string BillingProfileName { get; set; }
        [JsonPropertyName("accountOwnerId")]
        public string AccountOwnerId { get; set; }
        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }
        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; }
        [JsonPropertyName("subscriptionName")]
        public string SubscriptionName { get; set; }
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("product")]
        public string Product { get; set; }
        [JsonPropertyName("partNumber")]
        public string PartNumber { get; set; }
        [JsonPropertyName("meterId")]
        public string MeterId { get; set; }
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
        [JsonPropertyName("effectivePrice")]
        public decimal EffectivePrice { get; set; }
        [JsonPropertyName("cost")]
        public decimal Cost { get; set; }
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
        [JsonPropertyName("billingCurrency")]
        public string BillingCurrency { get; set; }
        [JsonPropertyName("resourceLocation")]
        public string ResourceLocation { get; set; }
        [JsonPropertyName("consumedService")]
        public string ConsumedService { get; set; }
        [JsonPropertyName("resourceId")]
        public string ResourceId { get; set; }
        [JsonPropertyName("resourceName")]
        public string ResourceName { get; set; }
        [JsonPropertyName("invoiceSection")]
        public string InvoiceSection { get; set; }
        [JsonPropertyName("resourceGroup")]
        public string ResourceGroup { get; set; }
        [JsonPropertyName("offerId")]
        public string OfferId { get; set; }
        [JsonPropertyName("isAzureCreditEligible")]
        public bool IsAzureCreditEligible { get; set; }
        [JsonPropertyName("publisherType")]
        public string PublisherType { get; set; }
        [JsonPropertyName("chargeType")]
        public string ChargeType { get; set; }
        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }
        [JsonPropertyName("meterDetails")]
        public object MeterDetails { get; set; }
    }
}