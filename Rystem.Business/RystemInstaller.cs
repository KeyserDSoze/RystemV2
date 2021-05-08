using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.Installation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rystem.Business
{
    public sealed class RystemInstaller
    {
        private sealed class ServiceCollection : IServiceCollection
        {
            public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<ServiceDescriptor> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public int IndexOf(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
        public static AzureBuilder WithAzure(IServiceCollection serviceCollection = default)
            => (serviceCollection ?? new ServiceCollection()).WithAzure();
    }
}