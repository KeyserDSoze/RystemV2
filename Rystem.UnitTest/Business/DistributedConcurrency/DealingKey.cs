using Rystem.Concurrency;

namespace Rystem.UnitTest.Business.DistributedConcurrency
{
    internal sealed class DealingKey : IDistributedConcurrencyKey
    {
        public string Key { get; init; }
    }
    internal sealed class SecondDealingKey : IConfigurableDistributedConcurrencyKey
    {
        public string Key { get; init; }

        public RystemDistributedServiceProvider Configure(string callerName)
        {
            return this.StartConfiguration()
                .WithAzure()
                .WithBlobStorage()
                .ConfigureAfterBuild();
        }
    }
}