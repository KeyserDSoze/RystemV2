using Azure.Messaging.EventHubs;
using Azure.Messaging.ServiceBus;
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
            var eventHub = config.GetSection("EventHub");
            var serviceBus = config.GetSection("ServiceBus");
            RystemInstaller
              .WithAzure()
              .AddStorage(new Azure.Integration.Storage.StorageOptions(storage["Name"], storage["Key"]))
              .AddMessage(new Azure.Integration.Message.EventHubOptions(eventHub["FullyQualifiedName"], eventHub["AccessKey"], new Azure.Integration.Storage.StorageOptions(storage["Name"], storage["Key"])))
              .AddMessage(new Azure.Integration.Message.ServiceBusOptions(serviceBus["FullyQualifiedName"], serviceBus["AccessKey"]))
              .Build();
        }
    }
}