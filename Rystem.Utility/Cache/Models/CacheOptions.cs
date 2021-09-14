using System;
using System.Collections.Generic;

namespace Rystem.Cache
{
    public class CacheOptions
    {
        public TimeSpan DefaultExpireAfter { get; set; }
        private List<(ICachePathFinder Finder, TimeSpan ExpireAfter)> PathFinders { get; } = new();
        public CacheOptions AddPath(ICachePathFinder pathFinder, TimeSpan expireAfter = default)
        {
            PathFinders.Add((pathFinder, expireAfter));
            return this;
        }
        public CacheOptions AddPath(StartingStringPathFinder pathFinder, TimeSpan expireAfter = default)
            => AddPath(pathFinder as ICachePathFinder, expireAfter);
        public CacheOptions AddPath(RegularExpressionPathFinder pathFinder, TimeSpan expireAfter = default)
            => AddPath(pathFinder as ICachePathFinder, expireAfter);
        public (bool IsMatch, TimeSpan ExpireAfter) IsMatching(CachedHttpMethod method, string uri)
        {
            foreach (var pathFinder in PathFinders)
            {
                if (pathFinder.Finder.Method.HasFlag(method) && pathFinder.Finder.IsMatching(uri))
                    return (true, pathFinder.ExpireAfter);
            }
            return (false, default);
        }
    }
}