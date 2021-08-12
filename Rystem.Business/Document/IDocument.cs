using System;

namespace Rystem.Business
{
    public interface IDocument
    {
        string PrimaryKey { get; set; }
        string SecondaryKey { get; set; }
        DateTime Timestamp { get; set; }
        RystemDocumentServiceProvider ConfigureDocument();
        internal RystemDocumentServiceProvider BuildDocument()
            => ConfigureDocument().AddInstance(this.GetType());
    }
}