namespace Rystem.Business
{
    public interface IConfigurableQueue : IQueue, IConfigurable<RystemQueueServiceProvider>
    {
    }
}