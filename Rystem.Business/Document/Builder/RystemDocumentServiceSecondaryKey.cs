using System.Linq.Expressions;
using System;

namespace Rystem.Business
{
    public sealed class RystemDocumentServiceSecondaryKey<T> : DocumentServiceKey<T>
    {
        public RystemDocumentServiceSecondaryKey(ServiceProvider rystemServiceProvider, RystemDocumentServiceProviderOptions options) : base(rystemServiceProvider, options)
        {
        }
        public RystemDocumentServiceTimestamp<T> WithSecondaryKey<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            Options.SecondaryKey = ((dynamic)navigationPropertyPath.Body).Member;
            return new RystemDocumentServiceTimestamp<T>(RystemDocumentServiceProvider, Options);
        }
    }
}