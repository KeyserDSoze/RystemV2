using Microsoft.Extensions.Configuration;
using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.UnitTest
{
    internal static class AzureConst
    {
        public static void Load()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var storage = config.GetSection("Storage");
            RystemInstaller
              .WithAzure()
              .AddStorage(new Azure.Integration.Storage.StorageOptions(storage["Name"], storage["Key"]))
              .Build();
        }
    }
}