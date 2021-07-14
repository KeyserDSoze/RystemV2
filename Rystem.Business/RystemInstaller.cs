using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.Installation;

namespace Rystem.Business
{
    public sealed class RystemInstaller
    {
        public static AzureBuilder WithAzure()
            => (null as IServiceCollection).WithAzure();
    }
}