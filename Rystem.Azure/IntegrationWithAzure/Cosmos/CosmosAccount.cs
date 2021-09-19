namespace Rystem.Azure.Integration.Cosmos
{
    /// <summary>
    /// Leave AccountKey empty if you want to connect through the managed identity.
    /// </summary>
    public sealed record CosmosAccount(string AccountName, string AccountKey = default, string DatabaseName = default) : IRystemOptions
    {
        public string ConnectionString
            => string.Format("AccountEndpoint=https://{0}.documents.azure.com:443/;AccountKey={1};", AccountName, AccountKey);
        public bool UseKeyVault { get; }
        public KeyVaultValue KeyVaultValue { get; }

        public CosmosAccount(KeyVaultValue keyVaultValue)
            : this(default, default)
        {
            KeyVaultValue = keyVaultValue;
            UseKeyVault = true;
        }
    }
}