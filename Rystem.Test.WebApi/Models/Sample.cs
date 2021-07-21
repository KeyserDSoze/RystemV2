using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Test.WebApi.Models
{
    public class Sample : IDocument
    {
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public DateTime Timestamp { get; set; }

        public RystemDocumentServiceProvider ConfigureDocument()
        {
            return RystemDocumentServiceProvider
                .WithAzure()
                .WithTableStorage();
        }
    }
}
