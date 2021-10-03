using System;

namespace Rystem
{
    public interface IConfigurable
    {
        public void Build(Action<string> action = default)
        {
            action?.Invoke(this.GetType().Name);
        }
    }
}