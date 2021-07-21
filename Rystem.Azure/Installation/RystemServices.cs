using Microsoft.Extensions.Logging;

namespace Rystem.Azure
{
    public class RystemServices
    {
        internal static AzureBuilder Builder;
        public AzureFactory Factory => Builder.Factory;
    }
}