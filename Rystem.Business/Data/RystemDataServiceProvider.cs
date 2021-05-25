namespace Rystem.Business
{
    public class RystemDataServiceProvider : ServiceProvider<RystemDataServiceProvider>
    {
        private RystemDataServiceProvider() { }
        public static AzureDataServiceBuilder WithAzure(Installation installation = Installation.Default)
          => new RystemDataServiceProvider().AndWithAzure(installation);
        public AzureDataServiceBuilder AndWithAzure(Installation installation = Installation.Default)
          => new(installation, this);
    }
}