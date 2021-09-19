using System.Linq.Expressions;
using System;

namespace Rystem.Business
{
    public class RystemDocumentServiceTimestamp<T> : DocumentServiceKey<T>
        where T : new()
    {
        public RystemDocumentServiceTimestamp(ServiceProvider rystemServiceProvider, RystemDocumentServiceProviderOptions options) : base(rystemServiceProvider, options)
        {
        }
        public RystemDocumentServiceProvider<T> WithTimestamp<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            Options.Timestamp = ((dynamic)navigationPropertyPath.Body).Member;
            return RystemDocumentServiceProvider;
        }
    }
}