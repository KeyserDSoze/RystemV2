using System;
using System.Collections.Generic;
using System.Linq;

namespace Rystem.Cloud
{
    public sealed record Tenant(string Name, List<Subscription> Subscriptions)
    {
        public Dictionary<string, List<string>> PossibleMetrics => Subscriptions.SelectMany(x => x.PossibleMetrics).Where(x => x.Value.Count > 0).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Where(t => t.Key == x.Key).FirstOrDefault().Value);
        public IEnumerable<string> Types => Subscriptions.SelectMany(x => x.Types).Distinct();
        public decimal Billed => Subscriptions.Sum(x => x.Billed);
        public decimal UsdBilled => Subscriptions.Sum(x => x.UsdBilled);
        public DateTime TheOldestDatetime => Subscriptions.SelectMany(x => x.ResourceGroups.SelectMany(t => t.Resources.SelectMany(q => q.Costs))).OrderByDescending(x => x.EventDate).FirstOrDefault()?.EventDate ?? new DateTime(1970, 1, 1);
        public void AddTenant(Tenant tenant)
        {
            var isOldest = tenant.TheOldestDatetime > TheOldestDatetime;
            foreach (var subscription in tenant.Subscriptions)
            {
                var actualSubscription = Subscriptions.FirstOrDefault(x => x.Id == subscription.Id);
                if (actualSubscription == default)
                    Subscriptions.Add(subscription);
                else
                {
                    foreach (var resourceGroup in subscription.ResourceGroups)
                    {
                        var actualResourceGroup = actualSubscription.ResourceGroups.FirstOrDefault(x => x.Id == resourceGroup.Id);
                        if (actualResourceGroup == default)
                            actualSubscription.ResourceGroups.Add(resourceGroup);
                        else
                        {
                            foreach (var resource in resourceGroup.Resources)
                            {
                                var actualResource = actualResourceGroup.Resources.FirstOrDefault(x => x.Id == resource.Id);
                                if (actualResource == default)
                                    actualResourceGroup.Resources.Add(resource);
                                else
                                {
                                    resource.Costs.AddRange(actualResource.Costs);
                                    resource.Monitorings.AddRange(actualResource.Monitorings);
                                    foreach (var metrics in actualResource.PossibleMetrics)
                                    {
                                        if (!resource.PossibleMetrics.Contains(metrics))
                                            resource.PossibleMetrics.Add(metrics);
                                    }
                                    AddToTags(resource.Tags, actualResource.Tags, isOldest);
                                }
                            }
                            AddToTags(resourceGroup.Tags, actualResourceGroup.Tags, isOldest);
                        }

                    }
                    AddToTags(subscription.Tags, actualSubscription.Tags, isOldest);
                }
            }
            static void AddToTags(Dictionary<string, string> subscription, Dictionary<string, string> actualSubscription, bool isOldest)
            {
                foreach (var tag in subscription)
                {
                    if (!actualSubscription.ContainsKey(tag.Key))
                    {
                        actualSubscription.Add(tag.Key, tag.Value);
                    }
                    else if (isOldest)
                    {
                        actualSubscription[tag.Key] = tag.Value;
                    }
                }
            }
        }
        public Dictionary<ConsumptionKey, List<Consumption>> GetConsumptions(bool withBillAccountAndOfferId = false, Dictionary<ConsumptionKey, List<Consumption>> consumptions = default)
        {
            if (consumptions == default)
                consumptions = new();
            foreach (var subscription in Subscriptions)
                _ = subscription.GetConsumptions(withBillAccountAndOfferId, consumptions);
            return consumptions;
        }
    }
}