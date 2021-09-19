namespace Rystem.Azure.Integration.Storage
{
    public sealed record QueueStorageConfiguration(string Name) : Configuration(Name)
    {
        public QueueStorageConfiguration() : this(string.Empty) { }
    }
}