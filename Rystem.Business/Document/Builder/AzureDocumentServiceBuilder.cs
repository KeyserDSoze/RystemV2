using Rystem.Azure.Installation;
using Rystem.Azure.IntegrationWithAzure.Storage;
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
        private readonly RystemServices RystemService;
        private readonly Installation Installation;
        public AzureDocumentServiceBuilder(Installation installation, RystemServices rystemService)
        {
            RystemService = rystemService;
            Installation = installation;
        }
        public RystemServices WithTableStorage(string tableName = null, string labelKey = null)
        {
            if (tableName == null)
                tableName = NameOfCallingClass();
            if (labelKey == null)
                labelKey = string.Empty;
            RystemService.Services.Add(Installation, new RystemService
            {
                Service = new TableStorageIntegration(tableName, AzureManager.Instance.Storages[labelKey]),
                Type = RystemServiceType.AzureTableStorage
            });
            return RystemService;
        }
        public static string NameOfCallingClass()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
                fullName = declaringType.Name;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return fullName;
        }
        public RystemServices WithBlob(string containerName = null, string labelKey = null, [CallerMemberName] string memberName = "")
        {
            if (labelKey == null)
                labelKey = string.Empty;
            RystemService.Services.Add(Installation, new RystemService
            {
                Service = new BlobStorageIntegration(containerName ?? memberName, AzureManager.Instance.Storages[labelKey]),
                Type = RystemServiceType.AzureBlobStorage
            });
            return RystemService;
        }
    }
}
