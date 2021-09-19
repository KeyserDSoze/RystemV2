using System.Linq.Expressions;
using System;

namespace Rystem.Business
{
    public class RystemDocumentServicePrimaryKey<T> : DocumentServiceKey<T>
        where T : new()
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