namespace Rystem.Azure.Integration.Storage
{
    public sealed record TableStorageConfiguration(string Name) : Configuration(Name)
    {
        public TableStorageConfiguration() : this(string.Empty) { }
    }
}