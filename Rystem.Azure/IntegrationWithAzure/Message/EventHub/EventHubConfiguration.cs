namespace Rystem.Azure.Integration.Message
{
    public sealed record EventHubConfiguration(string Name, string ConsumerGroup = default) : Configuration(Name)
    {
        public EventHubConfiguration() : this(string.Empty) { }
    }
}