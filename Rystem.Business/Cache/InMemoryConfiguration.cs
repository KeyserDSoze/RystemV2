using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business.Cache
{
    public sealed record InMemoryConfiguration(string Name, ExpireAfter ExpireAfter) : Configuration(Name)
    {
        public InMemoryConfiguration() : this(string.Empty, ExpireAfter.Infinite) { }
        public DateTime ExpiringTime => ExpireAfter == ExpireAfter.Infinite ? DateTime.MaxValue : DateTime.UtcNow.Add(new TimeSpan(0, 0, (int)ExpireAfter));
    }
}
