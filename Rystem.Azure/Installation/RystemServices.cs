using Microsoft.Extensions.Logging;
using System;

namespace Rystem.Azure
{
    public class RystemServices
    {
        internal static AzureBuilder AzureBuilder;
        public AzureFactory AzureFactory => AzureBuilder.Factory;
    }
}