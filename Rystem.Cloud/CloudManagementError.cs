using System;

namespace Rystem.Cloud
{
    public record CloudManagementError(string SubscriptionId, string Name, CloudManagementErrorType Type, Exception Exception);
    public enum CloudManagementErrorType
    {
        Authentication,
        Subscription,
        Subscriptions,
        ResourceGroup,
        Resource,
        Consumption,
        Cost,
        Metric,
    }
}