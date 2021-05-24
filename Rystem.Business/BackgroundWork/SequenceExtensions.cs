using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.BackgroundWork
{
    public static class SequenceExtensions
    {
        private static SequenceManager<TSequence, T> Manager<TSequence, T>(this TSequence entity)
            where TSequence : IAggregation<T>
            => entity.DefaultManager(nameof(SequenceExtensions), (key) => new SequenceManager<TSequence, T>(entity.BuildSequence())) as SequenceManager<TSequence, T>;

        public static void Add<T>(this IAggregation<T> key, T entity, Installation installation = Installation.Default)
            => key.Manager<IAggregation<T>, T>().Add(entity, installation);
        public static void Flush<T>(this IAggregation<T> key, Installation installation = Installation.Default)
            => key.Manager<IAggregation<T>, T>().Flush(installation);
    }
}