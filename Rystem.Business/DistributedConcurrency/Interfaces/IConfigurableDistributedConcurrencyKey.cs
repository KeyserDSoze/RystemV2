namespace Rystem.Concurrency
{
    public interface IConfigurableDistributedConcurrencyKey : IDistributedConcurrencyKey, IConfigurable<RystemDistributedServiceProvider>
    {
    }
}