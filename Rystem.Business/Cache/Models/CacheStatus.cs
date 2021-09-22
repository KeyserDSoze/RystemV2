namespace Rystem.Business
{
    internal sealed class CacheStatus<TCache>
    {
        public bool IsOk { get; }
        public TCache Cache { get; }
        public CacheStatus(bool isOk)
            => this.IsOk = isOk;
        public CacheStatus(bool isOk, TCache cache) : this(isOk)
            => this.Cache = cache;
        private static readonly CacheStatus<TCache> ok = new(true);
        private static readonly CacheStatus<TCache> notOk = new(false);
        public static CacheStatus<TCache> NotOk() => notOk;
        public static CacheStatus<TCache> Ok(TCache cache = default) => cache == null ? ok : new CacheStatus<TCache>(true, cache);
    }
}