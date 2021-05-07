﻿using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Azure.Integration.Secrets
{
    public sealed record KeyVaultOptions(string Url, SecretClientOptions SecretOptions, CertificateClientOptions CertificateOptions, KeyClientOptions KeyOptions);

    public sealed class KeyVaultIntegration
    {
        private readonly SecretClient SecretClient;
        private readonly CertificateClient CertificateClient;
        private readonly KeyClient KeyClient;
        public KeyVaultIntegration(KeyVaultOptions options)
        {
            SecretClient = new SecretClient(vaultUri: new Uri(options.Url), credential: new DefaultAzureCredential(), options: options.SecretOptions);
            CertificateClient = new CertificateClient(vaultUri: new Uri(options.Url), credential: new DefaultAzureCredential(), options: options.CertificateOptions);
            KeyClient = new KeyClient(vaultUri: new Uri(options.Url), credential: new DefaultAzureCredential(), options: options.KeyOptions);
        }
        public async Task<KeyVaultSecret> GetSecretAsync(string key, string version = null)
        {
            return (await SecretClient.GetSecretAsync(key, version)).Value;
        }
        public async Task SetSecretAsync(string key, string value)
        {
            _ = await SecretClient.SetSecretAsync(key, value);
        }
        public async Task<KeyVaultKey> GetKeyAsync(string key, string version = null)
        {
            return (await KeyClient.GetKeyAsync(key, version)).Value;
        }
        public async Task SetKeyAsync(string key, string value)
        {
            _ = await KeyClient.CreateKeyAsync(key, value);
        }
    }
}