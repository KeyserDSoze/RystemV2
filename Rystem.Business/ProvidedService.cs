using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    internal record ProvidedService(ServiceProviderType Type, dynamic Configurations, string ServiceKey, dynamic Options);
    public abstract class ServiceProvider<T>
        where T : ServiceProvider<T>
    {
        private const string Name = nameof(T);
        internal Dictionary<Installation, ProvidedService> Services { get; } = new();
        public Type InstanceType { get; private set; }
        internal T AddInstance(Type instanceType)
        {
            this.InstanceType = instanceType;
            return this as T;
        }
    }
}