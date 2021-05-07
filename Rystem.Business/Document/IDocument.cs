using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    public interface IDocument
    {
        RystemDocumentServiceProvider ConfigureDocument();
        internal RystemDocumentServiceProvider BuildDocument()
            => ConfigureDocument().AddInstance(this.GetType());
    }
}