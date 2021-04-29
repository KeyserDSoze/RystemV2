using Rystem.Azure.Installation;
using Rystem.Azure.IntegrationWithAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business
{
    public class AzureDataServiceBuilder
    {
        private readonly RystemServices RystemService;
        private readonly Installation Installation;
        public AzureDataServiceBuilder(Installation installation, RystemServices rystemService)
        {
            RystemService = rystemService;
            Installation = installation;
        }
        public RystemServices WithBlob(string containerName = null, string labelKey = null)
        {
            if (labelKey == null)
                labelKey = string.Empty;
            RystemService.Services.Add(Installation, new RystemService
            {
                Service = new BlobStorageIntegration(containerName, AzureManager.Instance.Storages[labelKey]),
                Type = RystemServiceType.AzureBlobStorage
            });
            return RystemService;
        }
    }
}