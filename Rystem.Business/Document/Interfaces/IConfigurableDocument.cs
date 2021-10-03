namespace Rystem.Business
{
    public interface IConfigurableDocument : IDocument, IConfigurable
    {
        RystemDocumentServiceProvider Configure(string callerName);
    }
}