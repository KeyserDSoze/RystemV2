using System.Text.RegularExpressions;

namespace Rystem.Cache
{
  
    public sealed record RegularExpressionPathFinder(CachedHttpMethod Method, Regex Regex) : ICachePathFinder
    {
        public bool IsMatching(string uri)
            => Regex.IsMatch(uri.ToLower());
    }
}