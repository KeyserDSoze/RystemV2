namespace Rystem.Azure.Integration.Message
{
    public sealed record ServiceBusConfiguration(string Name) : Configuration(Name)
    {
        public ServiceBusConfiguration() : this(string.Empty) { }
    }
}