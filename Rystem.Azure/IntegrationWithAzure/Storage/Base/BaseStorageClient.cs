namespace Rystem.Azure.Integration.Storage
{
    public abstract class BaseStorageClient
    {
        private protected StorageAccount Account;
        public BaseStorageClient(StorageAccount account) 
            => Account = account;
    }
}