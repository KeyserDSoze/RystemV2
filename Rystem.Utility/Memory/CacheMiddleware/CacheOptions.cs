using System.Collections.Generic;

namespace Rystem.Memory
{
    public class CacheOptions
    {
        private List<ICacheCheker> Checkers { get; } = new();
        public CacheOptions AddCheck(ICacheCheker checker)
        {
            Checkers.Add(checker);
            return this;
        }
        public CacheOptions AddCheck(StartingStringCacheChecker checker)
            => AddCheck(checker as ICacheCheker);
        public CacheOptions AddCheck(RegularExpressionCacheChecker checker)
            => AddCheck(checker as ICacheCheker);
        public bool IsMatching(CachedHttpMethod method, string uri)
        {
            foreach (var checker in Checkers)
            {
                if (checker.Method.HasFlag(method) && checker.IsMatching(uri))
                    return true;
            }
            return false;
        }
    }
}