using System.Linq.Expressions;
using System;

namespace Rystem.Business
{
    public sealed class RystemDocumentServicePrimaryKey<T> : DocumentServiceKey<T>
    {
        public RystemDocumentServicePrimaryKey(ServiceProvider rystemServiceProvider, RystemDocumentServiceProviderOptions options) : base(rystemServiceProvider, options)
        {   
        }
        public RystemDocumentServiceSecondaryKey<T> WithPrimaryKey<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            Options.PrimaryKey = ((dynamic)navigationPropertyPath.Body).Member;
            return new RystemDocumentServiceSecondaryKey<T>(RystemDocumentServiceProvider, Options);
        }
    }
}