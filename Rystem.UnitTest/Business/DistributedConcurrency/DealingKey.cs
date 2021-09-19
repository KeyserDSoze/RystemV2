using Rystem.Concurrency;

namespace Rystem.UnitTest.Business.DistributedConcurrency
{
    internal sealed class DealingKey : IDistributedConcurrencyKey
    {
        public string Key { get; init; }
    }
}