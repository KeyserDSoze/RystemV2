using System;
using System.Linq.Expressions;

namespace Rystem.Business
{
    public sealed class RystemDataServiceName<T>
    {
        private readonly RystemDataServiceProvider<T> RystemDataServiceProvider;
        private readonly RystemDataServiceProviderOptions Options;
        public RystemDataServiceName(ServiceProvider rystemServiceProvider, RystemDataServiceProviderOptions options)
        {
            RystemDataServiceProvider = (RystemDataServiceProvider<T>)rystemServiceProvider;
            Options = options;
        }
        public RystemDataServiceProvider<T> WithName<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            Options.Name = ((dynamic)navigationPropertyPath.Body).Member;
            return RystemDataServiceProvider;
        }
    }
}