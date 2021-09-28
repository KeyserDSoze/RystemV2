namespace Rystem.Business
{
    public sealed record ProvidedService(ServiceProviderType Type, dynamic Configurations, string ServiceKey, dynamic Options);
   
}