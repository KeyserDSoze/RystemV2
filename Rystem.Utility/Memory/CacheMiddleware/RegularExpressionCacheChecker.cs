using System.Text.RegularExpressions;

namespace Rystem.Memory
{
  
    public record RegularExpressionCacheChecker(CachedHttpMethod Method, Regex Regex) : ICacheCheker
    {
        public bool IsMatching(string uri)
            => Regex.IsMatch(uri.ToLower());
    }
}