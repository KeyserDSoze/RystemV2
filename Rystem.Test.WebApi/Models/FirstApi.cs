using Rystem.Azure.Integration.Storage;
using Rystem.Business;
using Rystem.FluentApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Test.WebApi.Models
{
    public class TenantData : IData
    {
        public string Name { get; set; }

        public RystemDataServiceProvider ConfigureData()
        {
            return RystemDataServiceProvider
                .WithAzure()
                .WithBlockBlob(new BlobStorageConfiguration("rawcost"));
        }
    }
    public class Moralta
    {
        
    }
    public class FirstApi : IFluentApi
    {
        public string Name => nameof(FirstApi);
        public async Task<bool> GetAsync()
        {
            await Task.Delay(0);
            return true;
        }
        [NoFluentApi]
        public async Task<Moralta> GetMoraltaAsync()
        {
            await Task.Delay(0);
            return new();
        }
    }
}
