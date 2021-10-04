namespace Rystem
{
    public sealed record ProvidedService(ServiceProviderType Type, dynamic Configurations, string ServiceKey, dynamic Options);
}