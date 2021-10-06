using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public sealed record SequenceProperty<T>(string Name = "", int MaximumBuffer = 5000, TimeSpan MaximumRetention = default, params Func<IEnumerable<T>, Task>[] Actions) : Configuration(Name)
    {
        public SequenceProperty() : this(string.Empty) { }
    }
}