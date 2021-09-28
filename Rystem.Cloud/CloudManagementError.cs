using System;

namespace Rystem.Cloud
{
    public sealed record CloudManagementError(string SubscriptionId, string Name, CloudManagementErrorType Type, Exception Exception)
    {
        public override string ToString()
            => $"{SubscriptionId}---{Name}---{Type}---{Exception}";
    }
}