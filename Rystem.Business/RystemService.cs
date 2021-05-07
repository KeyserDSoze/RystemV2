using System;
using System.Collections.Generic;

namespace Rystem.Business
{
    internal record RystemService(RystemServiceType Type, dynamic Configurations, string ServiceKey);
    public abstract class RystemServiceProvider<T>
        where T : RystemServiceProvider<T>
    {
        private const string Name = nameof(T);
        internal Dictionary<Installation, RystemService> Services { get; } = new();
        public Type InstanceType { get; private set; }
        internal T AddInstance(Type instanceType)
        {
            this.InstanceType = instanceType;
            return this as T;
        }
    }
}