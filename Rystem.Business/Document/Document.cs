using System;

namespace Rystem.Business
{
    public class Document<T> : IDocument
    {
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public DateTime Timestamp { get; set; }

        public RystemDocumentServiceProvider ConfigureDocument()
            => RystemDocumentServiceProvider
                .WithAzure()
                .WithTableStorage()
                .AndWithAzure(Installation.Inst00)
                .WithBlobStorage();
    }
}