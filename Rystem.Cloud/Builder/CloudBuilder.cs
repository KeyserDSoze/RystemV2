using Rystem.Business;
using Rystem.Cloud.Azure;
using System.Collections.Generic;

namespace Rystem.Cloud
{
    public class CloudBuilder
    {
        internal readonly Dictionary<Installation, dynamic> Values = new();
        private CloudBuilder() { }
        public static CloudBuilder Create()
            => new();
        public CloudBuilder WithAzure(AzureAadAppRegistration azureAadAppRegistration, Installation installation = Installation.Default)
        {
            Values.Add(installation, azureAadAppRegistration);
            return this;
        }
    }
}