using System.Reflection;

namespace Rystem.Business
{
    public class RystemDocumentServiceProviderOptions
    {
        public PropertyInfo PrimaryKey { get; set; }
        public PropertyInfo SecondaryKey { get; set; }
        public PropertyInfo Timestamp { get; set; }
    }
}