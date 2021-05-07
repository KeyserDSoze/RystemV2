using Rystem.Azure.Installation;
using Rystem.Azure.Integration.Storage;
using Rystem.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business
{
    public class AzureDocumentServiceBuilder
    {
        private readonly RystemDocumentServiceProvider RystemServiceProvider;
        private readonly Installation Installation;
        public AzureDocumentServiceBuilder(Installation installation, RystemDocumentServiceProvider rystemServiceProvider)
        {
            RystemServiceProvider = rystemServiceProvider;
            Installation = installation;
        }
        public RystemDocumentServiceProvider WithTableStorage(TableStorageConfiguration configuration = null, string serviceKey = null)
        {
            if (configuration == null)
                configuration = new TableStorageConfiguration(ReflectionHelper.NameOfCallingClass());
            else if (configuration.TableName != null)
                configuration = configuration with { TableName = ReflectionHelper.NameOfCallingClass() };
            RystemServiceProvider.Services.Add(Installation,
                new RystemService(RystemServiceType.AzureTableStorage, configuration, serviceKey ?? string.Empty));
            return RystemServiceProvider;
        }
        public RystemDocumentServiceProvider WithBlobStorage(BlobStorageConfiguration configuration = null, string serviceKey = null)
        {
            if (configuration == null)
                configuration = new BlobStorageConfiguration(ReflectionHelper.NameOfCallingClass());
            else if (configuration.ContainerName != null)
                configuration = configuration with { ContainerName = ReflectionHelper.NameOfCallingClass() };
            RystemServiceProvider.Services.Add(Installation,
                new RystemService(RystemServiceType.AzureBlobStorage, configuration, serviceKey ?? string.Empty));
            return RystemServiceProvider;
        }
    }
}
