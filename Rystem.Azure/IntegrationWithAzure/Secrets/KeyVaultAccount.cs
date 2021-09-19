using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace Rystem.Azure.Integration.Secrets
{
    public sealed record KeyVaultAccount(string Url, SecretClientOptions SecretOptions = default, CertificateClientOptions CertificateOptions = default, KeyClientOptions KeyOptions = default);
}