namespace Rystem.Cloud.Azure
{
    public sealed record AzureAadAppRegistration(string ClientId, string ClientSecret, string TenantId);
}